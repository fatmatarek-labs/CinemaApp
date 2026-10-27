using CinemaDashboard.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CinemaApp.Areas.Customer.Controllers;

[Area("Customer")]
public class CinemasController : Controller
{
    private readonly ApplicationDbContext _context;

    public CinemasController(ApplicationDbContext context) => _context = context;

    public async Task<IActionResult> Index() =>
        View(await _context.Cinemas.ToListAsync());

    public async Task<IActionResult> Details(int id)
    {
        var cinema = await _context.Cinemas.FirstOrDefaultAsync(c => c.Id == id);
        if (cinema == null) return NotFound();

        ViewBag.Cinema = cinema;
        var movies = await _context.Movies
            .Include(m => m.Category)
            .Where(m => m.CinemaId == id)
            .ToListAsync();

        return View(movies);
    }
}