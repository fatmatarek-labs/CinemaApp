using CinemaDashboard.Models;

namespace CinemaDashboard.Services;

public record BookingResult(bool Succeeded, Booking? Booking = null, string? Error = null, bool NotFound = false)
{
    public static BookingResult Ok(Booking booking) => new(true, booking);
    public static BookingResult Fail(string error) => new(false, Error: error);
    public static BookingResult Missing() => new(false, NotFound: true);
}