namespace PollBuilder.Domain.Entities;

public class Vote
{
    public Guid Id { get; set; }

    public Guid PollId { get; set; }

    public Guid PollOptionId { get; set; }

    public string IPAddress { get; set; } = string.Empty;

    public string FingerPrint { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public Poll Poll { get; set; } = null!;

    public PollOption PollOption { get; set; } = null!;
}