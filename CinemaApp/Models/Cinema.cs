namespace CinemaDashboard.Models;

public class Cinema
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Img { get; set; } = string.Empty;

    public List<Movie> Movies { get; set; } = new();
}