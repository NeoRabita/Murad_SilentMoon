using FluentValidation;

namespace SilentMoon.Application.Features.User.Commands.VerifyEmail
{
    public class VerifyEmailCommandValidator : AbstractValidator<VerifyEmailCommand>
    {
        public VerifyEmailCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email boş ola bilməz.")
                .EmailAddress().WithMessage("Email düzgün formatda deyil.");

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Doğrulama kodu boş ola bilməz.")
                .Length(6).WithMessage("Doğrulama kodu 6 simvol olmalıdır.");
        }
    }
}
