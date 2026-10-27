using CinemaDashboard.Data;
using CinemaDashboard.Models;
using CinemaDashboard.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CinemaApp.Areas.Customer.Controllers;

[Area("Customer")]
public class BookingController : Controller
{
    private readonly ApplicationDbContext _context;

    public BookingController(ApplicationDbContext context) => _context = context;

    private void LoadMovies(int? selected = null) =>
        ViewBag.Movies = new SelectList(_context.Movies, "Id", "Name", selected);

    private Task<Booking?> FindBooking(int id, string phone) =>
        _context.Bookings.Include(b => b.Movie)
                .FirstOrDefaultAsync(b => b.Id == id && b.Phone == phone);

    private async Task<bool> FormIsValid(BookingFormViewModel vm)
    {
        if (ModelState.IsValid && !await _context.Movies.AnyAsync(m => m.Id == vm.MovieId))
            ModelState.AddModelError(nameof(vm.MovieId), "This movie does not exist");

        if (ModelState.IsValid) return true;

        LoadMovies(vm.MovieId);
        return false;
    }

    public IActionResult Create(int? movieId)
    {
        LoadMovies(movieId);
        return View(new BookingFormViewModel { MovieId = movieId ?? 0 });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BookingFormViewModel vm)
    {
        if (!await FormIsValid(vm)) return View(vm);

        var booking = new Booking
        {
            CustomerName = vm.CustomerName,
            Phone = vm.Phone,
            NumberOfTickets = vm.NumberOfTickets,
            MovieId = vm.MovieId
        };
        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Details), new { id = booking.Id });
    }

    public async Task<IActionResult> Details(int id)
    {
        var booking = await _context.Bookings.Include(b => b.Movie)
                                    .FirstOrDefaultAsync(b => b.Id == id);
        return booking == null ? NotFound() : View(booking);
    }

    public IActionResult Find() => View();

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Find(int id, string phone)
    {
        if (await FindBooking(id, phone) == null)
        {
            ViewBag.Error = "No booking matches this number and phone.";
            return View();
        }
        return RedirectToAction(nameof(Edit), new { id, phone });
    }

  
    public async Task<IActionResult> Edit(int id, string phone)
    {
        var booking = await FindBooking(id, phone);
        if (booking == null) return NotFound();

        LoadMovies(booking.MovieId);
        return View(new BookingFormViewModel
        {
            Id = booking.Id,
            CustomerName = booking.CustomerName,
            Phone = booking.Phone,
            NumberOfTickets = booking.NumberOfTickets,
            MovieId = booking.MovieId
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(BookingFormViewModel vm)
    {
        var booking = await FindBooking(vm.Id, vm.Phone);
        if (booking == null) return NotFound();

        if (!await FormIsValid(vm)) return View(vm);

        booking.CustomerName = vm.CustomerName;
        booking.NumberOfTickets = vm.NumberOfTickets;
        booking.MovieId = vm.MovieId;
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Details), new { id = booking.Id });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(int id, string phone)
    {
        var booking = await FindBooking(id, phone);
        if (booking == null) return NotFound();

        _context.Bookings.Remove(booking);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Your booking was cancelled.";
        return RedirectToAction(nameof(Find));
    }
}