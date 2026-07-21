namespace PollBuilder.Web.ViewModels.Poll;

public class PollListVM
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ExpiredAt { get; set; }

    public bool IsClosed { get; set; }
}