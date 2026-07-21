using MediatR;
using PollBuilder.Application.Interfaces.Repositories;
using PollBuilder.Application.Interfaces.Services;
using PollBuilder.Domain.Entities;

namespace PollBuilder.Application.Features.Polls.Commands.CreatePoll;

public class CreatePollCommandHandler
    : IRequestHandler<CreatePollCommand, Guid>
{
    private readonly IPollRepository _repository;
    private readonly ICacheService _cache;

    public CreatePollCommandHandler(
        IPollRepository repository,
        ICacheService cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public async Task<Guid> Handle(
        CreatePollCommand request,
        CancellationToken cancellationToken)
    {
        var poll = new Poll
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            CreatedAt = DateTime.UtcNow,
            ExpiredAt = request.ExpiredAt!.Value,
            CreatedById = request.CreatedById,
            IsClosed = false
        };

        var order = 1;

        foreach (var option in request.Options)
        {
            poll.Options.Add(new PollOption
            {
                Id = Guid.NewGuid(),
                Content = option,
                DisplayOrder = order++
            });
        }

        // Lưu Poll vào Database
        await _repository.AddAsync(poll);
        await _repository.SaveChangesAsync();

        // Xóa cache danh sách Poll
        await _cache.RemoveAsync("polls_all");

        return poll.Id;
    }

    //private static string GenerateCode()
    //{
    //    const string chars =
    //        "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    //
    //    var random = new Random();
    //
    //    return new string(
    //        Enumerable.Range(0, 6)
    //            .Select(_ => chars[random.Next(chars.Length)])
    //            .ToArray());
    //}
}