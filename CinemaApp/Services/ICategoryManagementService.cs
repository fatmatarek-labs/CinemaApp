using CinemaDashboard.Models;
using CinemaDashboard.ViewModels;

namespace CinemaDashboard.Services;

public interface ICategoryManagementService
{
    Task<Category?> GetByIdAsync(int id);
    Task<Category> CreateAsync(CategoryFormViewModel vm);
    Task<bool> UpdateAsync(CategoryFormViewModel vm);
    Task<DeleteResult> DeleteAsync(int id);
}