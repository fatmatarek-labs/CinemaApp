using CinemaDashboard.Services;
using CinemaDashboard.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CinemaApp.Areas.Admin.Controllers;

[Area("Admin")]
public class CategoriesController : Controller
{
    private readonly ICategoryService _reader;
    private readonly ICategoryManagementService _manager;

    public CategoriesController(ICategoryService reader, ICategoryManagementService manager)
    {
        _reader = reader;
        _manager = manager;
    }

    public async Task<IActionResult> Index() => View(await _reader.GetAllAsync());

    public IActionResult Create() => View(new CategoryFormViewModel());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CategoryFormViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        await _manager.CreateAsync(vm);
        TempData["Success"] = "Category created.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var category = await _manager.GetByIdAsync(id);
        if (category == null) return NotFound();

        return View(new CategoryFormViewModel { Id = category.Id, Name = category.Name });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(CategoryFormViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);
        if (!await _manager.UpdateAsync(vm)) return NotFound();

        TempData["Success"] = "Category updated.";
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
                TempData["Error"] = "This category has movies, so it cannot be deleted.";
                break;
            default:
                TempData["Success"] = "Category deleted.";
                break;
        }

        return RedirectToAction(nameof(Index));
    }
}