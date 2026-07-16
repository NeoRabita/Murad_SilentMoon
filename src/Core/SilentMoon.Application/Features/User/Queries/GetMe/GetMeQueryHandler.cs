using Application.Abstractions.Messaging;
using SilentMoon.Application.Interfaces.Authentication;
using SilentMoon.Domain.Entities.SilentMoon.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace SilentMoon.Application.Features.User.Queries.GetMe
{
    public class GetMeQueryHandler : IQueryHandler<GetMeQuery, MeResponse>
    {
        private readonly IUow _uow;
        private readonly ICurrentUser _currentUser;

        public GetMeQueryHandler(IUow uow,ICurrentUser currentUser)
        {
            _uow = uow;
            _currentUser = currentUser;
        }

        public async Task<Result<MeResponse>> Handle(GetMeQuery query,CancellationToken ct)
        {
            var userRepo = _uow.GetRepository<ApplicationUser>();

            var user = await userRepo.GetByIdAsync(_currentUser.UserId, ct);

            if (user == null)
            {
                return Error.NotFound(
                    "User.NotFound",
                    "User not found");
            }

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
