using CinemaDashboard.Models;
using CinemaDashboard.ViewModels;

namespace CinemaDashboard.Services;

public interface IActorManagementService
{
    Task<Actor> CreateAsync(ActorFormViewModel vm);
    Task<bool> UpdateAsync(ActorFormViewModel vm);
    Task<DeleteResult> DeleteAsync(int id);
}