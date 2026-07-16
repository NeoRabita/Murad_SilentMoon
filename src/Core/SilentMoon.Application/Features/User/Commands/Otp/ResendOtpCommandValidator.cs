using FluentValidation;

namespace SilentMoon.Application.Features.Accounts.Commands.ResendOtp
{
    public class ResendOtpCommandValidator : AbstractValidator<ResendOtpCommand>
    {
        public ResendOtpCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();
        }
    }
}
