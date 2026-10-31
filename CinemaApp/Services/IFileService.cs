namespace CinemaDashboard.Services;

public interface IFileService
{
    Task<string> SaveImageAsync(IFormFile file, string folder);
    void DeleteImage(string? fileName, string folder);
}