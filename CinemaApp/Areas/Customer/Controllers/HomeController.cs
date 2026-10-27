using CinemaDashboard.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CinemaApp.Areas.Customer.Controllers;

[Area("Customer")]
public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context) => _context = context;

    public async Task<IActionResult> Index(string? search, int? categoryId)
    {
        var movies = _context.Movies.Include(m => m.Category).AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            movies = movies.Where(m => m.Name.Contains(search));
        if (categoryId != null)
            movies = movies.Where(m => m.CategoryId == categoryId);

        ViewBag.Categories = await _context.Categories.ToListAsync();
        ViewBag.CategoryId = categoryId;
        ViewBag.Search = search;
        return View(await movies.ToListAsync());
    }

    public async Task<IActionResult> Details(int id)
    {
        var movie = await _context.Movies
            .Include(m => m.Category).Include(m => m.Cinema)
            .FirstOrDefaultAsync(m => m.Id == id);

        return movie == null ? NotFound() : View(movie);
    }
}