using CinemaDashboard.Models;
using CinemaDashboard.ViewModels;

namespace CinemaDashboard.Services;

public interface IMovieManagementService
{
    Task<List<Movie>> GetAllWithDetailsAsync();
    Task<Movie?> GetForEditAsync(int id);
    Task<OperationResult> CreateAsync(MovieFormViewModel vm);
    Task<OperationResult> UpdateAsync(MovieFormViewModel vm);
    Task<DeleteResult> DeleteAsync(int id);
}