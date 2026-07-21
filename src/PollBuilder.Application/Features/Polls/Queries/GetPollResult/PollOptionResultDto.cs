namespace PollBuilder.Application.Features.Polls.Queries.GetPollResult;

public class PollOptionResultDto
{
    public Guid Id { get; set; }

    public string Content { get; set; } = string.Empty;

    public int VoteCount { get; set; }

    public double Percentage { get; set; }
}