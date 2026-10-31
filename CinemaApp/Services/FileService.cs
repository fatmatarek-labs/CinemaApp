namespace CinemaDashboard.Services;

public class FileService : IFileService
{
    private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
    private const long MaxSizeInBytes = 2 * 1024 * 1024; // 2 MB

    private readonly IWebHostEnvironment _env;

    public FileService(IWebHostEnvironment env) => _env = env;

    public async Task<string> SaveImageAsync(IFormFile file, string folder)
    {
        if (file is null || file.Length == 0)
            throw new ArgumentException("No file was uploaded.");

        if (file.Length > MaxSizeInBytes)
            throw new ArgumentException("File is too large (max 2 MB).");

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(extension) || !file.ContentType.StartsWith("image/"))
            throw new ArgumentException("Only JPG, PNG, or WEBP images are allowed.");

        var directory = Path.Combine(_env.WebRootPath, "images", folder);
        Directory.CreateDirectory(directory);

        var fileName = $"{Guid.NewGuid()}{extension}";
        var fullPath = Path.Combine(directory, fileName);

        await using var stream = new FileStream(fullPath, FileMode.Create);
        await file.CopyToAsync(stream);

        return fileName;
    }

    public void DeleteImage(string? fileName, string folder)
    {
        if (string.IsNullOrWhiteSpace(fileName)) return;

        var safeName = Path.GetFileName(fileName);
        var fullPath = Path.Combine(_env.WebRootPath, "images", folder, safeName);

        if (File.Exists(fullPath)) File.Delete(fullPath);
    }
}