
namespace CinemaDashboard.Models;

public class Booking
{
    public int Id { get; set; }

    public string CustomerName { get; set; } = string.Empty;

    public int NumberOfTickets { get; set; }

    public DateTime BookingDate { get; set; } = DateTime.Now;

    public int MovieId { get; set; }

    public Movie Movie { get; set; } = null!;
    public string Phone { get; set; } = string.Empty;
}
