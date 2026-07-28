using Application.Abstractions.Messaging;
using SilentMoon.Application.Interfaces.Authentication;
using SilentMoon.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace SilentMoon.Application.Features.Favorites.Commands.RemoveFavorite
{
    public class RemoveFavoriteCommandHandler : ICommandHandler<RemoveFavoriteCommand>
    {
        private readonly IUow _uow;
        private readonly ICurrentUser _currentUser;

        public RemoveFavoriteCommandHandler(IUow uow,ICurrentUser currentUser)
        {
            _uow = uow;
            _currentUser = currentUser;
        }

        public async Task<Result> Handle(RemoveFavoriteCommand command,CancellationToken ct)
        {
            var favoriteRepo = _uow.GetRepository<Favorite>();

            var favorite = await favoriteRepo.FirstOrDefaultAsync(
                x => x.ApplicationUserId == _currentUser.UserId && x.ContentId == command.ContentId,
                ct);

            if (favorite != null)
            {
                favoriteRepo.Delete(favorite);
            }

            return Result.Success();
        }
    }
}
