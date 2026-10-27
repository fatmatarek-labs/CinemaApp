using CinemaDashboard.Models;

namespace CinemaDashboard.ViewModels;

public class ChartPoint
{
    public string Label { get; set; } = "";
    public decimal Value { get; set; }
}

public class DashboardViewModel
{
    public int MoviesCount { get; set; }
    public int CategoriesCount { get; set; }
    public int CinemasCount { get; set; }
    public int ActorsCount { get; set; }
    public int BookingsCount { get; set; }
    public int TicketsSold { get; set; }
    public decimal Revenue { get; set; }

    public List<ChartPoint> TopMovies { get; set; } = new();
    public List<ChartPoint> RevenueByMovie { get; set; } = new();
    public List<ChartPoint> ByCinema { get; set; } = new();
    public List<ChartPoint> ByCategory { get; set; } = new();
    public List<Booking> RecentBookings { get; set; } = new();
    public List<Movie> LatestMovies { get; set; } = new();
}