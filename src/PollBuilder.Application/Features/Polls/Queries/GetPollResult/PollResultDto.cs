namespace PollBuilder.Application.Features.Polls.Queries.GetPollResult;

public class PollResultDto
{
    public Guid PollId { get; set; }

    public string Title { get; set; } = string.Empty;

    public int TotalVotes { get; set; }

    public List<PollOptionResultDto> Options { get; set; } = [];

    public string? Description { get; set; } = "";

    public DateTime CreatedAt { get; set; }

    public DateTime? ExpiredAt { get; set; }

    public bool IsClosed { get; set; }

    public bool CanManage { get; set; }

    public string CreatedById { get; set; } = "";
}