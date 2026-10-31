using CinemaDashboard.Models;

namespace CinemaDashboard.Services;

public interface ICinemaService
{
    Task<List<Cinema>> GetAllAsync();
    Task<Cinema?> GetByIdAsync(int id);
    Task<List<Movie>> GetMoviesAsync(int cinemaId);
}