using FluentValidation;

namespace PollBuilder.Application.Features.Polls.Commands.VotePoll;

public class VotePollValidator
    : AbstractValidator<VotePollCommand>
{
    public VotePollValidator()
    {
        RuleFor(x => x.PollId)
            .NotEmpty();

        RuleFor(x => x.PollOptionId)
            .NotEmpty();
    }
}