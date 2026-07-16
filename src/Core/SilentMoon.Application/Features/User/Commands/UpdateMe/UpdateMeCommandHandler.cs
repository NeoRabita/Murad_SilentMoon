using Application.Abstractions.Messaging;
using SilentMoon.Application.Interfaces.Authentication;
using SilentMoon.Domain.Entities.SilentMoon.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace SilentMoon.Application.Features.User.Commands.UpdateMe
{
    public class UpdateMeCommandHandler : ICommandHandler<UpdateMeCommand, MeResponse>
    {
        private readonly IUow _uow;
        private readonly ICurrentUser _currentUser;

        public UpdateMeCommandHandler(
            IUow uow,
            ICurrentUser currentUser)
        {
            _uow = uow;
            _currentUser = currentUser;
        }

        public async Task<Result<MeResponse>> Handle(
            UpdateMeCommand command,
            CancellationToken ct)
        {
            var userRepo = _uow.GetRepository<ApplicationUser>();

            var user = await userRepo.GetByIdAsync(_currentUser.UserId, ct);

            if (user == null)
            {
                return Error.NotFound(
                    "User.NotFound",
                    "User not found");
            }

            if (!string.IsNullOrWhiteSpace(command.FirstName))
            {
                user.FirstName = command.FirstName;
            }

            if (!string.IsNullOrWhiteSpace(command.LastName))
            {
                user.LastName = command.LastName;
            }

            if (command.AvatarUrl != null)
            {
                user.AvatarUrl = command.AvatarUrl;
            }

            userRepo.Update(user);

            await _uow.SaveChangesAsync(ct);

            return new MeResponse
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName,
                Email = user.Email,
                AvatarUrl = user.AvatarUrl
            };
        }
    }
}
