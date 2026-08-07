using Application.Abstractions.Messaging;

namespace SilentMoon.Application.Features.User.Commands.Resgister
{
    public class RegisterCommand : ICommand
    {
        public string Name { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }
    }
}
