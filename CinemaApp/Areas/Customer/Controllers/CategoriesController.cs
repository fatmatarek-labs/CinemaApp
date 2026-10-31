using CinemaDashboard.Services;
using Microsoft.AspNetCore.Mvc;

namespace CinemaApp.Areas.Customer.Controllers;

[Area("Customer")]
public class CategoriesController : Controller
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService) => _categoryService = categoryService;

    public async Task<IActionResult> Index() => View(await _categoryService.GetAllAsync());
}