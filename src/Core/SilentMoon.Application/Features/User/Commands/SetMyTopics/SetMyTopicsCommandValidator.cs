using FluentValidation;

namespace SilentMoon.Application.Features.User.Commands.SetMyTopics
{
    public class SetMyTopicsCommandValidator : AbstractValidator<SetMyTopicsCommand>
    {
        public SetMyTopicsCommandValidator()
        {
            RuleFor(x => x.TopicIds)
                .NotNull();
        }
    }
}
