using Application.Abstractions.Messaging;

namespace SilentMoon.Application.Features.User.Commands.ForgotPassword
{
    public class ForgotPasswordCommand : ICommand
    {
        public string Email { get; set; }
    }
}
