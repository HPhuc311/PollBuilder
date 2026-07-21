using MediatR;

namespace PollBuilder.Application.Features.Polls.Commands.ClosePoll;

public record ClosePollCommand(Guid PollId) : IRequest<bool>;