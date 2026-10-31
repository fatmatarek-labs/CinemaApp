using CinemaDashboard.Services;
using CinemaDashboard.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CinemaApp.Areas.Admin.Controllers;

[Area("Admin")]
public class CinemasController : Controller
{
    private readonly ICinemaService _reader;
    private readonly ICinemaManagementService _manager;

    public CinemasController(ICinemaService reader, ICinemaManagementService manager)
    {
        _reader = reader;
        _manager = manager;
    }

    public async Task<IActionResult> Index() => View(await _reader.GetAllAsync());

    public IActionResult Create() => View(new CinemaFormViewModel());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CinemaFormViewModel vm)
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

        TempData["Success"] = "Cinema created.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var cinema = await _reader.GetByIdAsync(id);
        if (cinema == null) return NotFound();

        return View(new CinemaFormViewModel
        {
            Id = cinema.Id,
            Name = cinema.Name,
            ExistingImg = cinema.Img
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(CinemaFormViewModel vm)
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

        TempData["Success"] = "Cinema updated.";
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
                TempData["Error"] = "This cinema has movies, so it cannot be deleted.";
                break;
            default:
                TempData["Success"] = "Cinema deleted.";
                break;
        }

        return RedirectToAction(nameof(Index));
    }
}