using FluentAssertions;
using Moq;
using PollBuilder.Application.Features.Polls.Commands.CreatePoll;
using PollBuilder.Application.Interfaces.Repositories;
using PollBuilder.Application.Interfaces.Services;
using PollBuilder.Domain.Entities;

namespace PollBuilder.Tests.Polls.Commands;

public class CreatePollCommandHandlerTests
{
    private readonly Mock<IPollRepository> _repositoryMock;
    private readonly Mock<ICacheService> _cacheMock;
    private readonly CreatePollCommandHandler _handler;

    public CreatePollCommandHandlerTests()
    {
        _repositoryMock = new Mock<IPollRepository>();
        _cacheMock = new Mock<ICacheService>();

        _handler = new CreatePollCommandHandler(
            _repositoryMock.Object,
            _cacheMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldCreatePoll_WhenRequestIsValid()
    {
        // Arrange
        var request = new CreatePollCommand
        {
            Title = "Favorite Language",
            Description = "Choose one",
            ExpiredAt = DateTime.UtcNow.AddDays(1),
            CreatedById = "user-1",
            Options = new List<string>
            {
                "C#",
                "Java"
            }
        };

        Poll? savedPoll = null;

        _repositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Poll>()))
            .Callback<Poll>(p => savedPoll = p)
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Should().NotBe(Guid.Empty);

        savedPoll.Should().NotBeNull();
        savedPoll!.Title.Should().Be(request.Title);
        savedPoll.Description.Should().Be(request.Description);
        savedPoll.CreatedById.Should().Be(request.CreatedById);
        savedPoll.IsClosed.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_ShouldCreateAllOptions()
    {
        // Arrange
        var request = new CreatePollCommand
        {
            Title = "Favorite Food",
            ExpiredAt = DateTime.UtcNow.AddDays(2),
            CreatedById = "user-1",
            Options = new List<string>
            {
                "Pizza",
                "Burger",
                "Sushi"
            }
        };

        Poll? savedPoll = null;

        _repositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Poll>()))
            .Callback<Poll>(p => savedPoll = p)
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        savedPoll.Should().NotBeNull();

        savedPoll!.Options.Should().HaveCount(3);

        savedPoll.Options.Select(x => x.Content)
            .Should()
            .Contain(new[]
            {
                "Pizza",
                "Burger",
                "Sushi"
            });

        savedPoll.Options.Select(x => x.DisplayOrder)
            .Should()
            .Equal(1, 2, 3);
    }

    [Fact]
    public async Task Handle_ShouldCallSaveChanges()
    {
        // Arrange
        var request = new CreatePollCommand
        {
            Title = "Question",
            ExpiredAt = DateTime.UtcNow.AddDays(1),
            CreatedById = "user",
            Options = new List<string>
            {
                "A",
                "B"
            }
        };

        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(
            x => x.AddAsync(It.IsAny<Poll>()),
            Times.Once);

        _repositoryMock.Verify(
            x => x.SaveChangesAsync(),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldClearCache()
    {
        // Arrange
        var request = new CreatePollCommand
        {
            Title = "Question",
            ExpiredAt = DateTime.UtcNow.AddDays(1),
            CreatedById = "user",
            Options = new List<string>
            {
                "Yes",
                "No"
            }
        };

        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        _cacheMock.Verify(
            x => x.RemoveAsync("polls_all"),
            Times.Once);
    }
}