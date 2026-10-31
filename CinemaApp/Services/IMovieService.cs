using CinemaDashboard.Models;

namespace CinemaDashboard.Services;

public interface IMovieService
{
    Task<List<Movie>> GetAllAsync();
    Task<bool> ExistsAsync(int id);
    Task<List<Movie>> SearchAsync(string? search, int? categoryId);
    Task<Movie?> GetDetailsAsync(int id);
}