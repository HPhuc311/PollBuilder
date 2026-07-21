using MediatR;

namespace PollBuilder.Application.Features.Polls.Queries.GetPollForDelete;

public record GetPollForDeleteQuery(Guid PollId)
    : IRequest<PollDeleteDto?>;