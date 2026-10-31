using System.ComponentModel.DataAnnotations;

namespace CinemaDashboard.ViewModels;

public class CinemaFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
    public string Name { get; set; } = string.Empty;

    public string? ExistingImg { get; set; }

    public IFormFile? ImageFile { get; set; }
}