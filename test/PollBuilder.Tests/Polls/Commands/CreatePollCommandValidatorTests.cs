using FluentAssertions;
using PollBuilder.Application.Features.Polls.Commands.CreatePoll;

namespace PollBuilder.Tests.Validators;

public class CreatePollCommandValidatorTests
{
    private readonly CreatePollCommandValidator _validator;

    public CreatePollCommandValidatorTests()
    {
        _validator = new CreatePollCommandValidator();
    }

    [Fact]
    public void ShouldFail_WhenTitleEmpty()
    {
        // Arrange
        var command = new CreatePollCommand
        {
            Title = "",
            Options = new List<string> { "A", "B" }
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();

        result.Errors.Should()
            .Contain(x => x.PropertyName == nameof(command.Title));
    }

    [Fact]
    public void ShouldFail_WhenLessThan2Options()
    {
        // Arrange
        var command = new CreatePollCommand
        {
            Title = "Favorite Language",
            Options = new List<string>
            {
                "C#"
            }
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();

        result.Errors.Should()
            .Contain(x => x.PropertyName == nameof(command.Options));
    }

    [Fact]
    public void ShouldFail_WhenMoreThan6Options()
    {
        // Arrange
        var command = new CreatePollCommand
        {
            Title = "Favorite Language",
            Options = new List<string>
            {
                "A",
                "B",
                "C",
                "D",
                "E",
                "F",
                "G"
            }
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();

        result.Errors.Should()
            .Contain(x => x.PropertyName == nameof(command.Options));
    }

    [Fact]
    public void ShouldPass_WhenValid()
    {
        // Arrange
        var command = new CreatePollCommand
        {
            Title = "Favorite Programming Language",
            Description = "Choose one option",
            Options = new List<string>
            {
                "C#",
                "Java",
                "Python"
            }
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}