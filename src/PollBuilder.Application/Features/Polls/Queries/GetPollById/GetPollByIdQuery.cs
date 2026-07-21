using MediatR;

namespace PollBuilder.Application.Features.Polls.Queries.GetPollById;

public record GetPollByIdQuery(Guid PollId)
    : IRequest<PollVoteDto?>;
