using CinemaDashboard.Data;
using CinemaDashboard.Models;
using CinemaDashboard.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace CinemaDashboard.Services;

public class MovieService : IMovieService, IMovieManagementService
{
    private const string ImageFolder = "movies";

    private readonly ApplicationDbContext _context;
    private readonly IFileService _fileService;

    public MovieService(ApplicationDbContext context, IFileService fileService)
    {
        _context = context;
        _fileService = fileService;
    }

    public async Task<List<Movie>> GetAllAsync() =>
        await _context.Movies.AsNoTracking().ToListAsync();

    public Task<bool> ExistsAsync(int id) =>
        _context.Movies.AnyAsync(m => m.Id == id);

    public async Task<List<Movie>> SearchAsync(string? search, int? categoryId)
    {
        var movies = _context.Movies.AsNoTracking().Include(m => m.Category).AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            movies = movies.Where(m => m.Name.Contains(search));
        if (categoryId != null)
            movies = movies.Where(m => m.CategoryId == categoryId);

        return await movies.ToListAsync();
    }

    public Task<Movie?> GetDetailsAsync(int id) =>
        _context.Movies
            .AsNoTracking()
            .Include(m => m.Category)
            .Include(m => m.Cinema)
            .FirstOrDefaultAsync(m => m.Id == id);

    public async Task<List<Movie>> GetAllWithDetailsAsync() =>
        await _context.Movies
            .AsNoTracking()
            .Include(m => m.Category)
            .Include(m => m.Cinema)
            .OrderBy(m => m.Name)
            .ToListAsync();

    public Task<Movie?> GetForEditAsync(int id) =>
        _context.Movies
            .AsNoTracking()
            .Include(m => m.MovieActors)
            .FirstOrDefaultAsync(m => m.Id == id);

    public async Task<OperationResult> CreateAsync(MovieFormViewModel vm)
    {
        var actorIds = vm.SelectedActorIds.Distinct().ToList();

        var error = await ValidateReferencesAsync(vm.CategoryId, vm.CinemaId, actorIds);
        if (error != null) return OperationResult.Fail(error);

        string imageName;
        try
        {
            imageName = await _fileService.SaveImageAsync(vm.MainImageFile!, ImageFolder);
        }
        catch (ArgumentException ex)
        {
            return OperationResult.Fail(ex.Message);
        }

        var movie = new Movie
        {
            Name = vm.Name.Trim(),
            Description = vm.Description.Trim(),
            Price = vm.Price,
            Status = vm.Status,
            ShowDateTime = vm.ShowDateTime,
            MainImg = imageName,
            CategoryId = vm.CategoryId,
            CinemaId = vm.CinemaId,
            MovieActors = actorIds.Select(id => new MovieActor { ActorId = id }).ToList()
        };

        try
        {
            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();
        }
        catch
        {
            _fileService.DeleteImage(imageName, ImageFolder);
            throw;
        }

        return OperationResult.Ok();
    }

    public async Task<OperationResult> UpdateAsync(MovieFormViewModel vm)
    {
        var movie = await _context.Movies
            .Include(m => m.MovieActors)
            .FirstOrDefaultAsync(m => m.Id == vm.Id);
        if (movie == null) return OperationResult.Missing();

        var actorIds = vm.SelectedActorIds.Distinct().ToList();

        var error = await ValidateReferencesAsync(vm.CategoryId, vm.CinemaId, actorIds);
        if (error != null) return OperationResult.Fail(error);

        string? newImage = null;
        if (vm.MainImageFile is { Length: > 0 })
        {
            try
            {
                newImage = await _fileService.SaveImageAsync(vm.MainImageFile, ImageFolder);
            }
            catch (ArgumentException ex)
            {
                return OperationResult.Fail(ex.Message);
            }
        }

        var oldImage = movie.MainImg;

        movie.Name = vm.Name.Trim();
        movie.Description = vm.Description.Trim();
        movie.Price = vm.Price;
        movie.Status = vm.Status;
        movie.ShowDateTime = vm.ShowDateTime;
        movie.CategoryId = vm.CategoryId;
        movie.CinemaId = vm.CinemaId;
        if (newImage != null) movie.MainImg = newImage;

        var toRemove = movie.MovieActors.Where(ma => !actorIds.Contains(ma.ActorId)).ToList();
        _context.MovieActors.RemoveRange(toRemove);

        var existingIds = movie.MovieActors.Select(ma => ma.ActorId).ToHashSet();
        foreach (var actorId in actorIds.Where(id => !existingIds.Contains(id)))
            movie.MovieActors.Add(new MovieActor { ActorId = actorId });

        try
        {
            await _context.SaveChangesAsync();
        }
        catch
        {
            if (newImage != null) _fileService.DeleteImage(newImage, ImageFolder);
            throw;
        }

        if (newImage != null) _fileService.DeleteImage(oldImage, ImageFolder);
        return OperationResult.Ok();
    }

    public async Task<DeleteResult> DeleteAsync(int id)
    {
        var movie = await _context.Movies.FindAsync(id);
        if (movie == null) return DeleteResult.NotFound;

        if (await _context.Bookings.AnyAsync(b => b.MovieId == id))
            return DeleteResult.InUse;

        var image = movie.MainImg;
        _context.Movies.Remove(movie);
        await _context.SaveChangesAsync();

        _fileService.DeleteImage(image, ImageFolder);
        return DeleteResult.Deleted;
    }

    private async Task<string?> ValidateReferencesAsync(int categoryId, int cinemaId, List<int> actorIds)
    {
        if (!await _context.Categories.AnyAsync(c => c.Id == categoryId))
            return "The selected category does not exist.";

        if (!await _context.Cinemas.AnyAsync(c => c.Id == cinemaId))
            return "The selected cinema does not exist.";

        var foundActors = await _context.Actors.CountAsync(a => actorIds.Contains(a.Id));
        if (foundActors != actorIds.Count)
            return "One or more selected actors do not exist.";

        return null;
    }
}