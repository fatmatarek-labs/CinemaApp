using CinemaDashboard.Models;
using CinemaDashboard.ViewModels;

namespace CinemaDashboard.Services;

public interface ICinemaManagementService
{
    Task<Cinema> CreateAsync(CinemaFormViewModel vm);
    Task<bool> UpdateAsync(CinemaFormViewModel vm);
    Task<DeleteResult> DeleteAsync(int id);
}