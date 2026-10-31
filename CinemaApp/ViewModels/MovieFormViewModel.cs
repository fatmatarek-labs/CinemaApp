using System.ComponentModel.DataAnnotations;
using CinemaDashboard.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CinemaDashboard.ViewModels;

public class MovieFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Name is required")]
    [StringLength(150, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 150 characters")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Description is required")]
    [StringLength(2000, ErrorMessage = "Description is too long (max 2000 characters)")]
    public string Description { get; set; } = string.Empty;

    [Range(0, 100000, ErrorMessage = "Price must be between 0 and 100000")]
    public decimal Price { get; set; }

    public MovieStatus Status { get; set; }

    [Display(Name = "Show date and time")]
    public DateTime ShowDateTime { get; set; } = DateTime.Now;

    [Range(1, int.MaxValue, ErrorMessage = "Please select a category")]
    [Display(Name = "Category")]
    public int CategoryId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Please select a cinema")]
    [Display(Name = "Cinema")]
    public int CinemaId { get; set; }

    public List<int> SelectedActorIds { get; set; } = new();

    public string? ExistingMainImg { get; set; }

    [Display(Name = "Main image")]
    public IFormFile? MainImageFile { get; set; }

    [ValidateNever] public List<SelectListItem> Categories { get; set; } = new();
    [ValidateNever] public List<SelectListItem> Cinemas { get; set; } = new();
    [ValidateNever] public List<SelectListItem> Actors { get; set; } = new();
}