using CinemaDashboard.Services;
using CinemaDashboard.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CinemaApp.Areas.Admin.Controllers;

[Area("Admin")]
public class ActorsController : Controller
{
    private readonly IActorService _reader;
    private readonly IActorManagementService _manager;

    public ActorsController(IActorService reader, IActorManagementService manager)
    {
        _reader = reader;
        _manager = manager;
    }

    public async Task<IActionResult> Index() => View(await _reader.GetAllAsync());

    public IActionResult Create() => View(new ActorFormViewModel());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ActorFormViewModel vm)
    {
        if (vm.ImageFile == null)
            ModelState.AddModelError(nameof(vm.ImageFile), "Image is required");
        if (!ModelState.IsValid) return View(vm);

        try
        {
            await _manager.CreateAsync(vm);
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(nameof(vm.ImageFile), ex.Message);
            return View(vm);
        }

        TempData["Success"] = "Actor created.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var actor = await _reader.GetByIdAsync(id);
        if (actor == null) return NotFound();

        return View(new ActorFormViewModel
        {
            Id = actor.Id,
            Name = actor.Name,
            ExistingImg = actor.Img
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ActorFormViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        try
        {
            if (!await _manager.UpdateAsync(vm)) return NotFound();
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(nameof(vm.ImageFile), ex.Message);
            return View(vm);
        }

        TempData["Success"] = "Actor updated.";
        return RedirectToAction(nameof(Index));
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
                TempData["Error"] = "This actor appears in movies, so he/she cannot be deleted.";
                break;
            default:
                TempData["Success"] = "Actor deleted.";
                break;
        }

        return RedirectToAction(nameof(Index));
    }
}