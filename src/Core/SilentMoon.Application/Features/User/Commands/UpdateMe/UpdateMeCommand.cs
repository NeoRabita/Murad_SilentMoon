using Application.Abstractions.Messaging;

namespace SilentMoon.Application.Features.User.Commands.UpdateMe
{
    public class UpdateMeCommand : ICommand<MeResponse>
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string AvatarUrl { get; set; }
    }
}
