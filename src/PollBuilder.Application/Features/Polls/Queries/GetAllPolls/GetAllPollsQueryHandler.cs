using MediatR;
using PollBuilder.Application.Interfaces.Repositories;
using PollBuilder.Application.Interfaces.Services;

namespace PollBuilder.Application.Features.Polls.Queries.GetAllPolls;

public class GetAllPollsQueryHandler
    : IRequestHandler<GetAllPollsQuery, List<PollDto>>
{
    private readonly IPollRepository _repository;
    private readonly ICacheService _cache;

    public GetAllPollsQueryHandler(
        IPollRepository repository,
        ICacheService cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public async Task<List<PollDto>> Handle(
        GetAllPollsQuery request,
        CancellationToken cancellationToken)
    {
        const string cacheKey = "polls_all";

        // 1. Kiểm tra dữ liệu trong Redis
        var cachedPolls =
            await _cache.GetAsync<List<PollDto>>(cacheKey);

        if (cachedPolls != null)
            return cachedPolls;

        // 2. Nếu chưa có thì lấy từ Database
        var polls = await _repository.GetAllAsync();

        var result = polls
            .Select(x => new PollDto
            {
                Id = x.Id,
                Title = x.Title,
                CreatedAt = x.CreatedAt,
                CreatedById = x.CreatedById,
                ExpiredAt = x.ExpiredAt,
                IsClosed = x.IsClosed
            })
            .ToList();

        // 3. Lưu vào Redis (5 phút)
        await _cache.SetAsync(
            cacheKey,
            result,
            TimeSpan.FromMinutes(5));

        return result;
    }
}