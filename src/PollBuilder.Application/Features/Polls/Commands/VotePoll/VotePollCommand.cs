using MediatR;

namespace PollBuilder.Application.Features.Polls.Commands.VotePoll;

public class VotePollCommand : IRequest<bool>
{
    public Guid PollId { get; set; }

    public Guid PollOptionId { get; set; }

    public string IPAddress { get; set; } = string.Empty;

    public string Fingerprint { get; set; } = string.Empty;
}