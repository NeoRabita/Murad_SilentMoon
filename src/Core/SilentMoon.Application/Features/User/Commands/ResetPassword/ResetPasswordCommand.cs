using Application.Abstractions.Messaging;

namespace SilentMoon.Application.Features.User.Commands.ResetPassword
{
    public class ResetPasswordCommand : ICommand
    {
        public string Email { get; set; }

        public string Code { get; set; }

        public string Password { get; set; }

        public string ConfirmPassword { get; set; }
    }
}
