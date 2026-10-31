using CinemaDashboard.Data;
using CinemaDashboard.Models;
using CinemaDashboard.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace CinemaDashboard.Services;

public class CinemaService : ICinemaService, ICinemaManagementService
{
    private const string ImageFolder = "cinemas";

    private readonly ApplicationDbContext _context;
    private readonly IFileService _fileService;

    public CinemaService(ApplicationDbContext context, IFileService fileService)
    {
        _context = context;
        _fileService = fileService;
    }

    public async Task<List<Cinema>> GetAllAsync() =>
        await _context.Cinemas.AsNoTracking().ToListAsync();

    public Task<Cinema?> GetByIdAsync(int id) =>
        _context.Cinemas.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);

    public async Task<List<Movie>> GetMoviesAsync(int cinemaId) =>
        await _context.Movies
            .AsNoTracking()
            .Include(m => m.Category)
            .Where(m => m.CinemaId == cinemaId)
            .ToListAsync();

    public async Task<Cinema> CreateAsync(CinemaFormViewModel vm)
    {
        var imageName = await _fileService.SaveImageAsync(vm.ImageFile!, ImageFolder);
        var cinema = new Cinema { Name = vm.Name.Trim(), Img = imageName };

        try
        {
            _context.Cinemas.Add(cinema);
            await _context.SaveChangesAsync();
        }
        catch
        {
            _fileService.DeleteImage(imageName, ImageFolder); 
            throw;
        }

        return cinema;
    }

    public async Task<bool> UpdateAsync(CinemaFormViewModel vm)
    {
        var cinema = await _context.Cinemas.FindAsync(vm.Id);
        if (cinema == null) return false;

        var oldImage = cinema.Img;
        string? newImage = null;

        if (vm.ImageFile is { Length: > 0 })
            newImage = await _fileService.SaveImageAsync(vm.ImageFile, ImageFolder);

        cinema.Name = vm.Name.Trim();
        if (newImage != null) cinema.Img = newImage;

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
        return true;
    }

    public async Task<DeleteResult> DeleteAsync(int id)
    {
        var cinema = await _context.Cinemas.FindAsync(id);
        if (cinema == null) return DeleteResult.NotFound;

        if (await _context.Movies.AnyAsync(m => m.CinemaId == id))
            return DeleteResult.InUse;

        var image = cinema.Img;
        _context.Cinemas.Remove(cinema);
        await _context.SaveChangesAsync();

        _fileService.DeleteImage(image, ImageFolder);
        return DeleteResult.Deleted;
    }
}