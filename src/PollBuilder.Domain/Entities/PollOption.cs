namespace PollBuilder.Domain.Entities;

public class PollOption
{
    public Guid Id { get; set; }

    public Guid PollId { get; set; }

    public string Content { get; set; } = string.Empty;

    public int DisplayOrder { get; set; }

    public Poll Poll { get; set; } = null!;

    public ICollection<Vote> Votes { get; set; } = new List<Vote>();
}