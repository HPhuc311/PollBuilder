using MediatR;
using PollBuilder.Application.Interfaces.Notifications;
using PollBuilder.Application.Interfaces.Repositories;
using PollBuilder.Application.Interfaces.Services;
using PollBuilder.Domain.Entities;

namespace PollBuilder.Application.Features.Polls.Commands.VotePoll;

public class VotePollHandler : IRequestHandler<VotePollCommand, bool>
{
    private readonly IPollRepository _pollRepository;
    private readonly IVoteRepository _voteRepository;
    private readonly IPollNotifier _pollNotifier;
    private readonly ICacheService _cache;

    public VotePollHandler(
        IPollRepository pollRepository,
        IVoteRepository voteRepository,
        IPollNotifier pollNotifier,
        ICacheService cache)
    {
        _pollRepository = pollRepository;
        _voteRepository = voteRepository;
        _pollNotifier = pollNotifier;
        _cache = cache;
    }

    public async Task<bool> Handle(
        VotePollCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Kiểm tra Poll có tồn tại không
        if (!await _pollRepository.PollExistsAsync(request.PollId))
            return false;

        // 2. Kiểm tra Poll còn mở không
        if (await _pollRepository.IsPollClosedAsync(request.PollId))
            return false;

        // 3. Kiểm tra Option có thuộc Poll không
        if (!await _pollRepository.OptionBelongsToPollAsync(
            request.PollId,
            request.PollOptionId))
            return false;

        // 4. Kiểm tra người dùng đã vote chưa (nếu sử dụng)
        /*
        if (await _voteRepository.HasUserVotedAsync(
            request.PollId,
            request.Fingerprint))
            return false;
        */

        // 5. Tạo Vote mới
        var vote = new Vote
        {
            Id = Guid.NewGuid(),
            PollId = request.PollId,
            PollOptionId = request.PollOptionId,
            IPAddress = request.IPAddress,
            FingerPrint = request.Fingerprint,
            CreatedAt = DateTime.UtcNow
        };

        // 6. Lưu Database
        await _voteRepository.AddAsync(vote);
        await _voteRepository.SaveChangesAsync();

        // 7. Xóa cache kết quả Poll
        await _cache.RemoveAsync($"poll_result_{request.PollId}");

        // Nếu sau này cache thêm Poll Detail thì xóa luôn
        // await _cache.RemoveAsync($"poll_detail_{request.PollId}");

        // 8. Cập nhật realtime qua SignalR
        await _pollNotifier.NotifyVoteUpdated(request.PollId);

        return true;
    }
}