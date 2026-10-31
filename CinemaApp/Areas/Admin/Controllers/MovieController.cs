using CinemaDashboard.Services;
using CinemaDashboard.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CinemaApp.Areas.Admin.Controllers;

[Area("Admin")]
public class MoviesController : Controller
{
    private readonly IMovieManagementService _manager;
    private readonly ICategoryService _categories;
    private readonly ICinemaService _cinemas;
    private readonly IActorService _actors;

    public MoviesController(
        IMovieManagementService manager,
        ICategoryService categories,
        ICinemaService cinemas,
        IActorService actors)
    {
        _manager = manager;
        _categories = categories;
        _cinemas = cinemas;
        _actors = actors;
    }

    private async Task FillListsAsync(MovieFormViewModel vm)
    {
        vm.Categories = (await _categories.GetAllAsync())
            .Select(c => new SelectListItem(c.Name, c.Id.ToString())).ToList();
        vm.Cinemas = (await _cinemas.GetAllAsync())
            .Select(c => new SelectListItem(c.Name, c.Id.ToString())).ToList();
        vm.Actors = (await _actors.GetAllAsync())
            .Select(a => new SelectListItem(a.Name, a.Id.ToString())).ToList();
    }

    public async Task<IActionResult> Index() => View(await _manager.GetAllWithDetailsAsync());

    public async Task<IActionResult> Create()
    {
        var vm = new MovieFormViewModel();
        await FillListsAsync(vm);
        return View(vm);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(MovieFormViewModel vm)
    {
        if (vm.MainImageFile == null)
            ModelState.AddModelError(nameof(vm.MainImageFile), "Main image is required");

        if (ModelState.IsValid)
        {
            var result = await _manager.CreateAsync(vm);
            if (result.Succeeded)
            {
                TempData["Success"] = "Movie created.";
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError(string.Empty, result.Error!);
        }

        await FillListsAsync(vm);
        return View(vm);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var movie = await _manager.GetForEditAsync(id);
        if (movie == null) return NotFound();

        var vm = new MovieFormViewModel
        {
            Id = movie.Id,
            Name = movie.Name,
            Description = movie.Description,
            Price = movie.Price,
            Status = movie.Status,
            ShowDateTime = movie.ShowDateTime,
            CategoryId = movie.CategoryId,
            CinemaId = movie.CinemaId,
            ExistingMainImg = movie.MainImg,
            SelectedActorIds = movie.MovieActors.Select(ma => ma.ActorId).ToList()
        };
        await FillListsAsync(vm);
        return View(vm);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(MovieFormViewModel vm)
    {
        if (ModelState.IsValid)
        {
            var result = await _manager.UpdateAsync(vm);
            if (result.NotFound) return NotFound();
            if (result.Succeeded)
            {
                TempData["Success"] = "Movie updated.";
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError(string.Empty, result.Error!);
        }

        await FillListsAsync(vm);
        return View(vm);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _manager.DeleteAsync(id);

        switch (result)
        {
            case DeleteResult.NotFound:
                return NotFound();
            case DeleteResult.InUse:
                TempData["Error"] = "This movie has bookings, so it cannot be deleted.";
                break;
            default:
                TempData["Success"] = "Movie deleted.";
                break;
        }

        return RedirectToAction(nameof(Index));
    }
}