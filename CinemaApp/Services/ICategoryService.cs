using CinemaDashboard.Models;

namespace CinemaDashboard.Services;

public interface ICategoryService
{
    Task<List<Category>> GetAllAsync();
}