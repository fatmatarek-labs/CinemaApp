using CinemaDashboard.Data;
using CinemaDashboard.Models;
using CinemaDashboard.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace CinemaDashboard.Services;

public class BookingService : IBookingService
{
    private const string MovieNotFoundMessage = "This movie does not exist";

    private readonly ApplicationDbContext _context;
    private readonly IMovieService _movieService;

    public BookingService(ApplicationDbContext context, IMovieService movieService)
    {
        _context = context;
        _movieService = movieService;
    }

    public Task<Booking?> GetByIdAsync(int id) =>
        _context.Bookings.AsNoTracking()
            .Include(b => b.Movie)
            .FirstOrDefaultAsync(b => b.Id == id);

    public Task<Booking?> FindForCustomerAsync(int id, string phone) =>
        _context.Bookings
            .Include(b => b.Movie)
            .FirstOrDefaultAsync(b => b.Id == id && b.Phone == phone);

    public async Task<BookingResult> CreateAsync(BookingFormViewModel vm)
    {
        if (!await _movieService.ExistsAsync(vm.MovieId))
            return BookingResult.Fail(MovieNotFoundMessage);

        var booking = new Booking
        {
            CustomerName = vm.CustomerName,
            Phone = vm.Phone,
            NumberOfTickets = vm.NumberOfTickets,
            MovieId = vm.MovieId
        };

        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        return BookingResult.Ok(booking);
    }

    public async Task<BookingResult> UpdateAsync(BookingFormViewModel vm)
    {
        var booking = await FindForCustomerAsync(vm.Id, vm.Phone);
        if (booking == null) return BookingResult.Missing();

        if (!await _movieService.ExistsAsync(vm.MovieId))
            return BookingResult.Fail(MovieNotFoundMessage);

        booking.CustomerName = vm.CustomerName;
        booking.NumberOfTickets = vm.NumberOfTickets;
        booking.MovieId = vm.MovieId;
        await _context.SaveChangesAsync();

        return BookingResult.Ok(booking);
    }

    public async Task<bool> CancelAsync(int id, string phone)
    {
        var booking = await FindForCustomerAsync(id, phone);
        if (booking == null) return false;

        _context.Bookings.Remove(booking);
        await _context.SaveChangesAsync();
        return true;
    }
}