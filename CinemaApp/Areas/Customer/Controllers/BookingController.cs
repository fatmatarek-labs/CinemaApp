using CinemaDashboard.Services;
using CinemaDashboard.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CinemaApp.Areas.Customer.Controllers;

[Area("Customer")]
public class BookingController : Controller
{
    private readonly IBookingService _bookingService;
    private readonly IMovieService _movieService;

    public BookingController(IBookingService bookingService, IMovieService movieService)
    {
        _bookingService = bookingService;
        _movieService = movieService;
    }

    private async Task LoadMoviesAsync(int? selected = null) =>
        ViewBag.Movies = new SelectList(await _movieService.GetAllAsync(), "Id", "Name", selected);

    public async Task<IActionResult> Create(int? movieId)
    {
        await LoadMoviesAsync(movieId);
        return View(new BookingFormViewModel { MovieId = movieId ?? 0 });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BookingFormViewModel vm)
    {
        if (ModelState.IsValid)
        {
            var result = await _bookingService.CreateAsync(vm);
            if (result.Succeeded)
                return RedirectToAction(nameof(Details), new { id = result.Booking!.Id });

            ModelState.AddModelError(nameof(vm.MovieId), result.Error!);
        }

        await LoadMoviesAsync(vm.MovieId);
        return View(vm);
    }

    public async Task<IActionResult> Details(int id)
    {
        var booking = await _bookingService.GetByIdAsync(id);
        return booking == null ? NotFound() : View(booking);
    }

    public IActionResult Find() => View();

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Find(int id, string phone)
    {
        if (await _bookingService.FindForCustomerAsync(id, phone) == null)
        {
            ViewBag.Error = "No booking matches this number and phone.";
            return View();
        }
        return RedirectToAction(nameof(Edit), new { id, phone });
    }

    public async Task<IActionResult> Edit(int id, string phone)
    {
        var booking = await _bookingService.FindForCustomerAsync(id, phone);
        if (booking == null) return NotFound();

        await LoadMoviesAsync(booking.MovieId);
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
        if (ModelState.IsValid)
        {
            var result = await _bookingService.UpdateAsync(vm);
            if (result.NotFound) return NotFound();
            if (result.Succeeded)
                return RedirectToAction(nameof(Details), new { id = result.Booking!.Id });

            ModelState.AddModelError(nameof(vm.MovieId), result.Error!);
        }

        await LoadMoviesAsync(vm.MovieId);
        return View(vm);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(int id, string phone)
    {
        if (!await _bookingService.CancelAsync(id, phone)) return NotFound();

        TempData["Success"] = "Your booking was cancelled.";
        return RedirectToAction(nameof(Find));
    }
}