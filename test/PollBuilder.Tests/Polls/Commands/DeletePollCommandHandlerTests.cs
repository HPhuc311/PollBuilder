using Moq;
using PollBuilder.Application.Features.Polls.Commands.DeletePoll;
using PollBuilder.Application.Interfaces.Repositories;
using PollBuilder.Application.Interfaces.Services;

namespace PollBuilder.Tests.Polls.Commands;

public class DeletePollCommandHandlerTests
{
    private readonly Mock<IPollRepository> _repository;
    private readonly Mock<ICacheService> _cache;
    private readonly DeletePollCommandHandler _handler;

    public DeletePollCommandHandlerTests()
    {
        _repository = new Mock<IPollRepository>();
        _cache = new Mock<ICacheService>();

        _handler = new DeletePollCommandHandler(
            _repository.Object,
            _cache.Object);
    }

    [Fact]
    public async Task Handle_ShouldDeletePoll()
    {
        // Arrange
        var pollId = Guid.NewGuid();

        var command = new DeletePollCommand(pollId);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _repository.Verify(
            x => x.DeleteAsync(pollId),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldRemoveCache()
    {
        // Arrange
        var pollId = Guid.NewGuid();

        var command = new DeletePollCommand(pollId);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _cache.Verify(
            x => x.RemoveAsync("polls_all"),
            Times.Once);

        _cache.Verify(
            x => x.RemoveAsync($"poll_result_{pollId}"),
            Times.Once);
    }
}