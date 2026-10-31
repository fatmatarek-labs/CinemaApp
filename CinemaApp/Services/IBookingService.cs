using CinemaDashboard.Models;
using CinemaDashboard.ViewModels;

namespace CinemaDashboard.Services;

public interface IBookingService
{
    Task<Booking?> GetByIdAsync(int id);
    Task<Booking?> FindForCustomerAsync(int id, string phone);
    Task<BookingResult> CreateAsync(BookingFormViewModel vm);
    Task<BookingResult> UpdateAsync(BookingFormViewModel vm);
    Task<bool> CancelAsync(int id, string phone);
}