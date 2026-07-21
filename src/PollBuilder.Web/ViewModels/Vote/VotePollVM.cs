using System.ComponentModel.DataAnnotations;

namespace PollBuilder.Web.ViewModels.Vote;

public class VotePollVM
{
    public Guid PollId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public List<VoteOptionVM> Options { get; set; } = [];

    [Required(ErrorMessage = "Please choose one option.")]
    public Guid SelectedOptionId { get; set; }

    public bool IsClosed { get; set; }

    public string FingerPrint { get; set; } = string.Empty;
}

public class VoteOptionVM
{
    public Guid Id { get; set; }

    public string Content { get; set; } = string.Empty;
}