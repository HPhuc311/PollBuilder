using MediatR;

namespace PollBuilder.Application.Features.Polls.Queries.GetPollResult;

public record GetPollResultQuery(Guid PollId)
    : IRequest<PollResultDto?>;