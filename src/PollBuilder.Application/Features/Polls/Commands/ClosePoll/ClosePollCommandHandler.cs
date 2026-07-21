using MediatR;
using PollBuilder.Application.Interfaces.Repositories;
using PollBuilder.Application.Interfaces.Services;

namespace PollBuilder.Application.Features.Polls.Commands.ClosePoll;

public class ClosePollCommandHandler
    : IRequestHandler<ClosePollCommand, bool>
{
    private readonly IPollRepository _pollRepository;
    private readonly ICacheService _cache;

    public ClosePollCommandHandler(
        IPollRepository pollRepository,
        ICacheService cache)
    {
        _pollRepository = pollRepository;
        _cache = cache;
    }

    public async Task<bool> Handle(
        ClosePollCommand request,
        CancellationToken cancellationToken)
    {
        var poll = await _pollRepository.GetByIdAsync(request.PollId);

        if (poll == null)
            return false;

        poll.IsClosed = true;

        await _pollRepository.SaveChangesAsync();

        // Xóa cache danh sách Poll
        await _cache.RemoveAsync("polls_all");

        // Xóa cache kết quả Poll
        await _cache.RemoveAsync($"poll_result_{request.PollId}");

        return true;
    }
}