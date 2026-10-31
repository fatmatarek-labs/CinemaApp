using CinemaDashboard.Services;
using Microsoft.AspNetCore.Mvc;

namespace CinemaApp.Areas.Customer.Controllers;

[Area("Customer")]
public class HomeController : Controller
{
    private readonly IMovieService _movieService;
    private readonly ICategoryService _categoryService;

    public HomeController(IMovieService movieService, ICategoryService categoryService)
    {
        _movieService = movieService;
        _categoryService = categoryService;
    }

    public async Task<IActionResult> Index(string? search, int? categoryId)
    {
        ViewBag.Categories = await _categoryService.GetAllAsync();
        ViewBag.CategoryId = categoryId;
        ViewBag.Search = search;
        return View(await _movieService.SearchAsync(search, categoryId));
    }

    public async Task<IActionResult> Details(int id)
    {
        var movie = await _movieService.GetDetailsAsync(id);
        return movie == null ? NotFound() : View(movie);
    }
}