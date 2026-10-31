using CinemaDashboard.Services;
using Microsoft.AspNetCore.Mvc;

namespace CinemaDashboard.Areas.Customer.Controllers;

[Area("Customer")]
public class ActorsController : Controller
{
    private readonly IActorService _actorService;

    public ActorsController(IActorService actorService) => _actorService = actorService;

    public async Task<IActionResult> Index() => View(await _actorService.GetAllAsync());
}