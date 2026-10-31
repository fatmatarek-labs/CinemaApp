using CinemaDashboard.Data;
using CinemaDashboard.Models;
using CinemaDashboard.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace CinemaDashboard.Services;

public class ActorService : IActorService, IActorManagementService
{
    private const string ImageFolder = "actors";

    private readonly ApplicationDbContext _context;
    private readonly IFileService _fileService;

    public ActorService(ApplicationDbContext context, IFileService fileService)
    {
        _context = context;
        _fileService = fileService;
    }

    public async Task<List<Actor>> GetAllAsync() =>
        await _context.Actors.AsNoTracking().OrderBy(a => a.Name).ToListAsync();

    public Task<Actor?> GetByIdAsync(int id) =>
        _context.Actors.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);

    public async Task<Actor> CreateAsync(ActorFormViewModel vm)
    {
        var imageName = await _fileService.SaveImageAsync(vm.ImageFile!, ImageFolder);
        var actor = new Actor { Name = vm.Name.Trim(), Img = imageName };

        try
        {
            _context.Actors.Add(actor);
            await _context.SaveChangesAsync();
        }
        catch
        {
            _fileService.DeleteImage(imageName, ImageFolder);
            throw;
        }

        return actor;
    }

    public async Task<bool> UpdateAsync(ActorFormViewModel vm)
    {
        var actor = await _context.Actors.FindAsync(vm.Id);
        if (actor == null) return false;

        var oldImage = actor.Img;
        string? newImage = null;

        if (vm.ImageFile is { Length: > 0 })
            newImage = await _fileService.SaveImageAsync(vm.ImageFile, ImageFolder);

        actor.Name = vm.Name.Trim();
        if (newImage != null) actor.Img = newImage;

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
        var actor = await _context.Actors.FindAsync(id);
        if (actor == null) return DeleteResult.NotFound;

        if (await _context.MovieActors.AnyAsync(ma => ma.ActorId == id))
            return DeleteResult.InUse;

        var image = actor.Img;
        _context.Actors.Remove(actor);
        await _context.SaveChangesAsync();

        _fileService.DeleteImage(image, ImageFolder);
        return DeleteResult.Deleted;
    }
}