using CinemaDashboard.Models;

namespace CinemaDashboard.Services;

public interface IActorService
{
    Task<List<Actor>> GetAllAsync();
    Task<Actor?> GetByIdAsync(int id);
}