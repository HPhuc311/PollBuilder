using FluentAssertions;
using PollBuilder.Application.Features.Polls.Commands.VotePoll;

namespace PollBuilder.Tests.Validators;

public class VotePollValidatorTests
{
    private readonly VotePollValidator _validator;

    public VotePollValidatorTests()
    {
        _validator = new VotePollValidator();
    }

    [Fact]
    public void ShouldFail_WhenPollIdIsEmpty()
    {
        // Arrange
        var command = new VotePollCommand
        {
            PollId = Guid.Empty,
            PollOptionId = Guid.NewGuid()
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();

        result.Errors.Should()
            .Contain(x => x.PropertyName == nameof(command.PollId));
    }

    [Fact]
    public void ShouldFail_WhenPollOptionIdIsEmpty()
    {
        // Arrange
        var command = new VotePollCommand
        {
            PollId = Guid.NewGuid(),
            PollOptionId = Guid.Empty
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();

        result.Errors.Should()
            .Contain(x => x.PropertyName == nameof(command.PollOptionId));
    }

    [Fact]
    public void ShouldPass_WhenCommandIsValid()
    {
        // Arrange
        var command = new VotePollCommand
        {
            PollId = Guid.NewGuid(),
            PollOptionId = Guid.NewGuid(),
            IPAddress = "127.0.0.1",
            Fingerprint = "browser-fingerprint"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}