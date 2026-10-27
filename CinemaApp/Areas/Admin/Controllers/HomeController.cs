using CinemaDashboard.Data;
using CinemaDashboard.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CinemaDashboard.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var dashboard = new DashboardViewModel
            {
                MoviesCount = await _context.Movies.CountAsync(),
                CategoriesCount = await _context.Categories.CountAsync(),
                CinemasCount = await _context.Cinemas.CountAsync(),
                ActorsCount = await _context.Actors.CountAsync(),
                BookingsCount = await _context.Bookings.CountAsync(),
                TicketsSold = await _context.Bookings.SumAsync(b => (int?)b.NumberOfTickets) ?? 0,
                Revenue = await _context.Bookings.SumAsync(b => (decimal?)(b.Movie.Price * b.NumberOfTickets)) ?? 0,

                TopMovies = await _context.Bookings
                    .GroupBy(b => b.Movie.Name)
                    .Select(g => new ChartPoint { Label = g.Key, Value = (decimal)g.Sum(b => b.NumberOfTickets) })
                    .OrderByDescending(x => x.Value)
                    .Take(6)
                    .ToListAsync(),

                RevenueByMovie = await _context.Bookings
                    .GroupBy(b => b.Movie.Name)
                    .Select(g => new ChartPoint { Label = g.Key, Value = g.Sum(b => b.Movie.Price * b.NumberOfTickets) })
                    .OrderByDescending(x => x.Value)
                    .Take(6)
                    .ToListAsync(),

                ByCinema = await _context.Movies
                    .GroupBy(m => m.Cinema.Name)
                    .Select(g => new ChartPoint { Label = g.Key, Value = (decimal)g.Count() })
                    .ToListAsync(),

                ByCategory = await _context.Movies
                    .GroupBy(m => m.Category.Name)
                    .Select(g => new ChartPoint { Label = g.Key, Value = (decimal)g.Count() })
                    .ToListAsync(),

                RecentBookings = await _context.Bookings
                    .Include(b => b.Movie)
                    .OrderByDescending(b => b.Id)
                    .Take(6)
                    .ToListAsync(),

                LatestMovies = await _context.Movies
                    .Include(m => m.Category)
                    .Include(m => m.Cinema)
                    .OrderByDescending(m => m.ShowDateTime)
                    .Take(5)
                    .ToListAsync()
            };

            return View(dashboard);
        }
    }
}