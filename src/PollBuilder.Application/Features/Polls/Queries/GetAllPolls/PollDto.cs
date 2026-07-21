namespace PollBuilder.Application.Features.Polls.Queries.GetAllPolls;

public class PollDto
{
    public Guid Id { get; set; }

    public string Title { get; set; }

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ExpiredAt { get; set; }

    public bool IsClosed { get; set; }
    public string CreatedById { get; set; } = string.Empty;


}