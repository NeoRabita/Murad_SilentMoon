using FluentValidation;

namespace SilentMoon.Application.Features.User.Commands.Resgister
{
    public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
    {
        public RegisterCommandValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("Ad boş ola bilməz.")
                .MaximumLength(50).WithMessage("Ad 50 simvoldan çox ola bilməz.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Soyad boş ola bilməz.")
                .MaximumLength(50).WithMessage("Soyad 50 simvoldan çox ola bilməz.");

            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("İstifadəçi adı boş ola bilməz.")
                .MinimumLength(3).WithMessage("İstifadəçi adı ən az 3 simvol olmalıdır.")
                .MaximumLength(30).WithMessage("İstifadəçi adı 30 simvoldan çox ola bilməz.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email boş ola bilməz.")
                .EmailAddress().WithMessage("Email düzgün formatda deyil.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Şifrə boş ola bilməz.")
                .MinimumLength(6).WithMessage("Şifrə ən az 6 simvol olmalıdır.")
                .MaximumLength(100).WithMessage("Şifrə 100 simvoldan çox ola bilməz.");

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage("Şifrə təkrarı boş ola bilməz.")
                .Equal(x => x.Password).WithMessage("Şifrələr uyğun gəlmir.");
        }
    }
}
