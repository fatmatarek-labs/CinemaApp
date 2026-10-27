using System.ComponentModel.DataAnnotations;

namespace CinemaDashboard.ViewModels;

public class BookingFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Name must be 3 to 100 characters")]
    [Display(Name = "Your Name")]
    public string CustomerName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone is required")]
    [RegularExpression(@"^\+?\d{10,15}$", ErrorMessage = "Enter a valid phone number (10 to 15 digits)")]
    public string Phone { get; set; } = string.Empty;

    [Range(1, 10, ErrorMessage = "Tickets must be between 1 and 10")]
    [Display(Name = "Number of Tickets")]
    public int NumberOfTickets { get; set; } = 1;

    [Range(1, int.MaxValue, ErrorMessage = "Please choose a movie")]
    [Display(Name = "Movie")]
    public int MovieId { get; set; }
}