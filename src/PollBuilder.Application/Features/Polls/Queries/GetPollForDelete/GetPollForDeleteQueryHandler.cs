using MediatR;
using PollBuilder.Application.Interfaces.Repositories;

namespace PollBuilder.Application.Features.Polls.Queries.GetPollForDelete;

public class GetPollForDeleteQueryHandler
    : IRequestHandler<GetPollForDeleteQuery, PollDeleteDto?>
{
    private readonly IPollRepository _repository;

    public GetPollForDeleteQueryHandler(IPollRepository repository)
    {
        _repository = repository;
    }

    public async Task<PollDeleteDto?> Handle(
        GetPollForDeleteQuery request,
        CancellationToken cancellationToken)
    {
        var poll = await _repository.GetByIdAsync(request.PollId);

        if (poll == null)
            return null;

        return new PollDeleteDto
        {
            Id = poll.Id,
            Title = poll.Title,
            Description = poll.Description,
            ExpiredAt = poll.ExpiredAt
        };
    }
}