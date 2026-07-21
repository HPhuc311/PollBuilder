using MediatR;
using PollBuilder.Application.Interfaces.Repositories;
using PollBuilder.Application.Interfaces.Services;

namespace PollBuilder.Application.Features.Polls.Queries.GetPollResult;

public class GetPollResultHandler
    : IRequestHandler<GetPollResultQuery, PollResultDto?>
{
    private readonly IPollRepository _repository;
    private readonly ICacheService _cache;

    public GetPollResultHandler(
        IPollRepository repository,
        ICacheService cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public async Task<PollResultDto?> Handle(
        GetPollResultQuery request,
        CancellationToken cancellationToken)
    {
        // Cache Key theo từng Poll
        string cacheKey = $"poll_result_{request.PollId}";

        // 1. Kiểm tra Redis trước
        var cachedResult =
            await _cache.GetAsync<PollResultDto>(cacheKey);

        if (cachedResult != null)
            return cachedResult;

        // 2. Nếu chưa có thì lấy Database
        var poll = await _repository.GetByIdAsync(request.PollId);

        if (poll == null)
            return null;

        var totalVotes = poll.Options.Sum(x => x.Votes.Count);

        var dto = new PollResultDto
        {
            PollId = poll.Id,
            Title = poll.Title,
            Description = poll.Description,
            CreatedAt = poll.CreatedAt,
            ExpiredAt = poll.ExpiredAt,
            IsClosed = poll.IsClosed,
            CreatedById = poll.CreatedById,
            TotalVotes = totalVotes
        };

        foreach (var option in poll.Options)
        {
            var voteCount = option.Votes.Count;

            dto.Options.Add(new PollOptionResultDto
            {
                Id = option.Id,
                Content = option.Content,
                VoteCount = voteCount,
                Percentage = totalVotes == 0
                    ? 0
                    : voteCount * 100.0 / totalVotes
            });
        }

        // 3. Lưu Redis (5 phút)
        await _cache.SetAsync(
            cacheKey,
            dto,
            TimeSpan.FromMinutes(5));

        return dto;
    }
}