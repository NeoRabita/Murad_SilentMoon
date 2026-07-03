using FluentValidation;

namespace SilentMoon.Application.Features.User.Commands.GoogleLogin
{
    public class GoogleLoginCommandValidator : AbstractValidator<GoogleLoginCommand>
    {
        public GoogleLoginCommandValidator()
        {
            RuleFor(x => x.IdToken)
                .NotEmpty().WithMessage("Google Id Token boş ola bilməz.");
        }
    }
}
