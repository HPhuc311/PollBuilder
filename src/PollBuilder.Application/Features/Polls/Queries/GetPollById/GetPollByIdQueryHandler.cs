using MediatR;
using PollBuilder.Application.Interfaces.Repositories;

namespace PollBuilder.Application.Features.Polls.Queries.GetPollById;

public class GetPollByIdQueryHandler
    : IRequestHandler<GetPollByIdQuery, PollVoteDto?>
{
    private readonly IPollRepository _repository;

    public GetPollByIdQueryHandler(IPollRepository repository)
    {
        _repository = repository;
    }

    public async Task<PollVoteDto?> Handle(
        GetPollByIdQuery request,
        CancellationToken cancellationToken)
    {
        var poll = await _repository.GetByIdAsync(request.PollId);

        if (poll == null)
            return null;

        return new PollVoteDto
        {
            Id = poll.Id,

            Title = poll.Title,

            Description = poll.Description,

            IsClosed = poll.IsClosed,

            Options = poll.Options
                .Select(x => new OptionDto
                {
                    Id = x.Id,

                    Content = x.Content
                })
                .ToList()
        };
    }
}