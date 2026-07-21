using MediatR;
using PollBuilder.Application.Interfaces.Repositories;
using PollBuilder.Application.Interfaces.Services;

namespace PollBuilder.Application.Features.Polls.Commands.DeletePoll;

public class DeletePollCommandHandler
    : IRequestHandler<DeletePollCommand>
{
    private readonly IPollRepository _repository;
    private readonly ICacheService _cache;

    public DeletePollCommandHandler(
        IPollRepository repository,
        ICacheService cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public async Task Handle(
        DeletePollCommand request,
        CancellationToken cancellationToken)
    {
        await _repository.DeleteAsync(request.Id);

        // Xóa cache danh sách Poll
        await _cache.RemoveAsync("polls_all");

        // Xóa cache kết quả Poll
        await _cache.RemoveAsync($"poll_result_{request.Id}");
    }
}