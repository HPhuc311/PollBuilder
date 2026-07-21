using MediatR;

namespace PollBuilder.Application.Features.Polls.Commands.CreatePoll;

public class CreatePollCommand : IRequest<Guid>
{
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime? ExpiredAt { get; set; }

    public List<string> Options { get; set; } = [];

    public string CreatedById { get; set; } = string.Empty;
}