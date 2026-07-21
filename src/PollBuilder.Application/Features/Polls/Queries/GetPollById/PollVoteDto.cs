namespace PollBuilder.Application.Features.Polls.Queries.GetPollById;

public class PollVoteDto
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsClosed { get; set; }

    public List<OptionDto> Options { get; set; } = [];
}

public class OptionDto
{
    public Guid Id { get; set; }

    public string Content { get; set; } = string.Empty;
}