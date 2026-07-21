namespace PollBuilder.Application.Features.Polls.Queries.GetPollById;

public class PollDetailVM
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime ExpiredAt { get; set; }
}