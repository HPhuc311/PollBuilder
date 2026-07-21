using FluentValidation;

namespace PollBuilder.Application.Features.Polls.Commands.CreatePoll;

public class CreatePollCommandValidator
    : AbstractValidator<CreatePollCommand>
{
    public CreatePollCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Options)
            .Must(x => x.Count >= 2)
            .WithMessage("At least 2 options are required.");

        RuleFor(x => x.Options)
            .Must(x => x.Count <= 6)
            .WithMessage("Maximum 6 options.");
    }
}