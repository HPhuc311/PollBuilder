using System.ComponentModel.DataAnnotations;

namespace PollBuilder.Web.ViewModels.Poll;

public class CreatePollVM
{
    [Required(ErrorMessage = "Question is required.")]
    [StringLength(
        200,
        ErrorMessage = "Question cannot exceed 200 characters."
    )]
    [Display(Name = "Question")]
    public string Title { get; set; } = string.Empty;

    [Display(Name = "Description")]
    [StringLength(
        1000,
        ErrorMessage = "Description cannot exceed 1000 characters."
    )]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Option 1 is required.")]
    [StringLength(
        100,
        ErrorMessage = "Option 1 cannot exceed 100 characters."
    )]
    [Display(Name = "Option 1")]
    public string Option1 { get; set; } = string.Empty;

    [Required(ErrorMessage = "Option 2 is required.")]
    [StringLength(
        100,
        ErrorMessage = "Option 2 cannot exceed 100 characters."
    )]
    [Display(Name = "Option 2")]
    public string Option2 { get; set; } = string.Empty;

    [StringLength(
        100,
        ErrorMessage = "Option 3 cannot exceed 100 characters."
    )]
    [Display(Name = "Option 3")]
    public string? Option3 { get; set; }

    [StringLength(
        100,
        ErrorMessage = "Option 4 cannot exceed 100 characters."
    )]
    [Display(Name = "Option 4")]
    public string? Option4 { get; set; }

    [StringLength(
        100,
        ErrorMessage = "Option 5 cannot exceed 100 characters."
    )]
    [Display(Name = "Option 5")]
    public string? Option5 { get; set; }

    [StringLength(
        100,
        ErrorMessage = "Option 6 cannot exceed 100 characters."
    )]
    [Display(Name = "Option 6")]
    public string? Option6 { get; set; }

    public string? ActiveOptionalOptions { get; set; }

    [Display(Name = "Expiration Date")]
    [DataType(DataType.DateTime)]
    public DateTime? ExpiredAt { get; set; }
}