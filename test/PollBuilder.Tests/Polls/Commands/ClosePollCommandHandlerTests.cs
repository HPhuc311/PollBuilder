using FluentAssertions;
using Moq;
using PollBuilder.Application.Features.Polls.Commands.ClosePoll;
using PollBuilder.Application.Interfaces.Repositories;
using PollBuilder.Application.Interfaces.Services;
using PollBuilder.Domain.Entities;

namespace PollBuilder.Tests.Polls.Commands;

public class ClosePollCommandHandlerTests
{
    private readonly Mock<IPollRepository> _pollRepository;
    private readonly Mock<ICacheService> _cacheService;
    private readonly ClosePollCommandHandler _handler;

    public ClosePollCommandHandlerTests()
    {
        _pollRepository = new Mock<IPollRepository>();
        _cacheService = new Mock<ICacheService>();

        _handler = new ClosePollCommandHandler(
            _pollRepository.Object,
            _cacheService.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnFalse_WhenPollNotFound()
    {
        // Arrange
        var pollId = Guid.NewGuid();

        _pollRepository
            .Setup(x => x.GetByIdAsync(pollId))
            .ReturnsAsync((Poll?)null);

        var command = new ClosePollCommand(PollId: pollId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeFalse();

        _pollRepository.Verify(
            x => x.SaveChangesAsync(),
            Times.Never);

        _cacheService.Verify(
            x => x.RemoveAsync(It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldClosePollSuccessfully()
    {
        // Arrange
        var poll = new Poll
        {
            Id = Guid.NewGuid(),
            Title = "Favorite Language",
            IsClosed = false
        };

        _pollRepository
            .Setup(x => x.GetByIdAsync(poll.Id))
            .ReturnsAsync(poll);

        var command = new ClosePollCommand(poll.Id);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();

        poll.IsClosed.Should().BeTrue();

        _pollRepository.Verify(
            x => x.SaveChangesAsync(),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldRemoveCache()
    {
        // Arrange
        var poll = new Poll
        {
            Id = Guid.NewGuid(),
            Title = "Poll",
            IsClosed = false
        };

        _pollRepository
            .Setup(x => x.GetByIdAsync(poll.Id))
            .ReturnsAsync(poll);

        var command = new ClosePollCommand(poll.Id);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _cacheService.Verify(
            x => x.RemoveAsync("polls_all"),
            Times.Once);

        _cacheService.Verify(
            x => x.RemoveAsync($"poll_result_{poll.Id}"),
            Times.Once);
    }
}