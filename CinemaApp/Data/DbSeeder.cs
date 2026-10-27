using CinemaDashboard.Models;

namespace CinemaDashboard.Data;

public static class DbSeeder
{
    private static string Img(string id) =>
        $"https://images.unsplash.com/photo-{id}?auto=format&fit=crop&w=700&q=85";

    private static readonly (string Name, string Category, decimal Price, string Image)[] Seeds =
    {
        ("The Shawshank Redemption", "Drama",   120, Img("1500530855697-b586d89ba3ee")),
        ("The Godfather",            "Drama",   130, Img("1470252649378-9c29740c9fa8")),
        ("The Dark Knight",          "Action",  150, Img("1519608487953-e999c86e7455")),
        ("Inception",                "Sci-Fi",  140, Img("1462331940025-496dfbfc7564")),
        ("Interstellar",             "Sci-Fi",  150, Img("1446776811953-b23d57bd21aa")),
        ("Titanic",                  "Romance", 110, Img("1472396961693-142e6e269027")),
        ("Avatar",                   "Sci-Fi",  160, Img("1500534623283-312aade485b7")),
        ("Forrest Gump",             "Drama",   100, Img("1500530855697-b586d89ba3ee"))
    };

    public static void Seed(ApplicationDbContext db)
    {
        bool tableWasEmpty = !db.Movies.Any();

        var cinema = db.Cinemas.FirstOrDefault()
            ?? db.Cinemas.Add(new Cinema { Name = "Fatoma's Cinema" }).Entity;
        db.SaveChanges();

        foreach (var s in Seeds)
        {
            var category = db.Categories.FirstOrDefault(c => c.Name == s.Category)
                ?? db.Categories.Add(new Category { Name = s.Category }).Entity;
            db.SaveChanges();

            var movie = db.Movies.FirstOrDefault(m => m.Name == s.Name);

            if (movie == null && tableWasEmpty)
            {
                db.Movies.Add(new Movie
                {
                    Name = s.Name,
                    Price = s.Price,
                    ShowDateTime = DateTime.Now.AddDays(3),
                    MainImg = s.Image,
                    CategoryId = category.Id,
                    CinemaId = cinema.Id
                });
            }
            else if (movie?.MainImg == "default.jpg")
            {
                movie.MainImg = s.Image;
                movie.CategoryId = category.Id;
            }
        }
        db.SaveChanges();
    }
}