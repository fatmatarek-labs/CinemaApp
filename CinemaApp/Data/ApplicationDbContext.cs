
using CinemaDashboard.Models;
using Microsoft.EntityFrameworkCore;

namespace CinemaDashboard.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Cinema> Cinemas => Set<Cinema>();
    public DbSet<Actor> Actors => Set<Actor>();
    public DbSet<Movie> Movies => Set<Movie>();
    public DbSet<MovieSubImg> MovieSubImgs => Set<MovieSubImg>();
    public DbSet<MovieActor> MovieActors => Set<MovieActor>();
    public DbSet<Booking> Bookings => Set<Booking>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MovieActor>()
            .HasKey(ma => new { ma.MovieId, ma.ActorId });

        modelBuilder.Entity<Movie>(entity =>
        {
            entity.Property(m => m.Price).HasPrecision(18, 2);
            entity.Property(m => m.Status).HasConversion<string>();

            entity.HasOne(m => m.Category)
                  .WithMany(c => c.Movies)
                  .HasForeignKey(m => m.CategoryId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(m => m.Cinema)
                  .WithMany(c => c.Movies)
                  .HasForeignKey(m => m.CinemaId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Booking>()
            .HasOne(b => b.Movie)
            .WithMany()
            .HasForeignKey(b => b.MovieId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
