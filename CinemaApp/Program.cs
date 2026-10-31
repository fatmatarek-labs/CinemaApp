using CinemaDashboard.Data;
using CinemaDashboard.Services;
using Microsoft.EntityFrameworkCore;

namespace CinemaApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddScoped<IActorService, ActorService>();
            builder.Services.AddScoped<IFileService, FileService>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<ICinemaService, CinemaService>();
            builder.Services.AddScoped<IMovieService, MovieService>();
            builder.Services.AddScoped<IBookingService, BookingService>();
            builder.Services.AddScoped<ICategoryManagementService, CategoryService>();
            builder.Services.AddScoped<ICinemaManagementService, CinemaService>();
            builder.Services.AddScoped<IActorManagementService, ActorService>();
            builder.Services.AddScoped<IMovieManagementService, MovieService>();

            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                DbSeeder.Seed(db);
            }

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}",
                defaults: new { area = "Customer" });

            app.Run();
        }
    }
}