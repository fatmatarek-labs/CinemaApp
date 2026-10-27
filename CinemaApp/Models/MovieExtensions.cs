using System.Globalization;

namespace CinemaDashboard.Models;

public static class MovieExtensions
{
    public static string PosterUrl(this Movie m) =>
        string.IsNullOrEmpty(m.MainImg) ? "/images/movies/default.jpg"
        : m.MainImg.StartsWith("http") ? m.MainImg
        : "/images/movies/" + m.MainImg;

    public static string Rating(this Movie m) =>
        (7.0 + m.Id * 7 % 20 / 10.0).ToString("0.0", CultureInfo.InvariantCulture);
}