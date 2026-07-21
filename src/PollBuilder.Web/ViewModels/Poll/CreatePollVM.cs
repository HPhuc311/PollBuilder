using System.ComponentModel.DataAnnotations;

namespace PollBuilder.Web.ViewModels.Poll;

public class CreatePollVM
{
    [Required(ErrorMessage = "Question is required.")]
    [StringLength(200)]
    [Display(Name = "Question")]
    public string Title { get; set; } = string.Empty;

    [Display(Name = "Description")]
    [StringLength(1000)]
    public string? Description { get; set; }

    [StringLength(100, ErrorMessage = "Option cannot exceed 100 characters.")]
    [Required(ErrorMessage = "Option is required.")]
    [Display(Name = "Option 1")]
    public string Option1 { get; set; } = string.Empty;

    [StringLength(100, ErrorMessage = "Option cannot exceed 100 characters.")]
    [Required(ErrorMessage = "Option is required.")]
    [Display(Name = "Option 2")]
    public string Option2 { get; set; } = string.Empty;

    [StringLength(100, ErrorMessage = "Option cannot exceed 100 characters.")]
    [Display(Name = "Option 3")]
    public string? Option3 { get; set; }

    [StringLength(100, ErrorMessage = "Option cannot exceed 100 characters.")]
    [Display(Name = "Option 4")]
    public string? Option4 { get; set; }

    [StringLength(100, ErrorMessage = "Option cannot exceed 100 characters.")]
    [Display(Name = "Option 5")]
    public string? Option5 { get; set; }

    [StringLength(100, ErrorMessage = "Option cannot exceed 100 characters.")]
    [Display(Name = "Option 6")]
    public string? Option6 { get; set; }

    [Display(Name = "Expiration Date")]
    [DataType(DataType.DateTime)]
    public DateTime? ExpiredAt { get; set; }
}