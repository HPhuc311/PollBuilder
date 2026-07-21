using MediatR;

namespace PollBuilder.Application.Features.Polls.Commands.DeletePoll;

public record DeletePollCommand(Guid Id) : IRequest;