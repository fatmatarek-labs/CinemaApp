namespace CinemaDashboard.Models;

public class Movie
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public MovieStatus Status { get; set; }
    public DateTime ShowDateTime { get; set; }
    public string MainImg { get; set; } = string.Empty;

    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public int CinemaId { get; set; }
    public Cinema Cinema { get; set; } = null!;

    public List<MovieSubImg> SubImages { get; set; } = new();
    public List<MovieActor> MovieActors { get; set; } = new();
}