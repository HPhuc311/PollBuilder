namespace PollBuilder.Web.ViewModels.Result;

public class PollResultVM
{
    public Guid PollId { get; set; }

    public string Title { get; set; } = string.Empty;

    public int TotalVotes { get; set; }

    public List<PollOptionVM> Options { get; set; } = [];

    public string? Description { get; set; } = "";

    public DateTime CreatedAt { get; set; }

    public DateTime? ExpiredAt { get; set; }

    public bool IsClosed { get; set; }

    public string CreatedById { get; set; } = "";
    public bool CanManage { get; set; }
}

public class PollOptionVM
{
    public string Content { get; set; } = string.Empty;

    public int VoteCount { get; set; }

    public double Percentage { get; set; }
}