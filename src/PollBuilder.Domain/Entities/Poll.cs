namespace PollBuilder.Domain.Entities;

public class Poll
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ExpiredAt { get; set; }

    public bool IsClosed { get; set; }

    public string CreatedById { get; set; } = string.Empty;

    public ICollection<PollOption> Options { get; set; } = new List<PollOption>();

    public ICollection<Vote> Votes { get; set; } = new List<Vote>();
}