using MediatR;

namespace PollBuilder.Application.Features.Polls.Queries.GetAllPolls;

public record GetAllPollsQuery : IRequest<List<PollDto>>;