using FluentValidation;

namespace SilentMoon.Application.Features.User.Commands.UpdateMe
{
    public class UpdateMeCommandValidator : AbstractValidator<UpdateMeCommand>
    {
        public UpdateMeCommandValidator()
        {
            RuleFor(x => x.FirstName)
                .MaximumLength(50)
                .When(x => x.FirstName != null);

            RuleFor(x => x.LastName)
                .MaximumLength(50)
                .When(x => x.LastName != null);

            RuleFor(x => x.AvatarUrl)
                .MaximumLength(2048)
                .When(x => x.AvatarUrl != null);
        }
    }
}
