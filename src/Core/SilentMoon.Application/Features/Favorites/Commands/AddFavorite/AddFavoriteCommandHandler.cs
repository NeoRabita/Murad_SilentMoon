using Application.Abstractions.Messaging;
using SilentMoon.Application.Interfaces.Authentication;
using SilentMoon.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace SilentMoon.Application.Features.Favorites.Commands.AddFavorite
{
    public class AddFavoriteCommandHandler : ICommandHandler<AddFavoriteCommand>
    {
        private readonly IUow _uow;
        private readonly ICurrentUser _currentUser;

        public AddFavoriteCommandHandler(IUow uow,ICurrentUser currentUser)
        {
            _uow = uow;
            _currentUser = currentUser;
        }

        public async Task<Result> Handle(AddFavoriteCommand command,CancellationToken ct)
        {
            var contentRepo = _uow.GetRepository<Content>();

            var content = await contentRepo.GetByIdAsync(command.ContentId, ct);

            if (content == null)
            {
                return Error.NotFound(
                    "Content.NotFound",
                    "Content not found");
            }

            var favoriteRepo = _uow.GetRepository<Favorite>();

            var existing = await favoriteRepo.FirstOrDefaultAsync(
                x => x.ApplicationUserId == _currentUser.UserId && x.ContentId == command.ContentId,
                ct);

            if (existing != null)
            {
                return Result.Success();
            }

            await favoriteRepo.AddAsync(new Favorite
            {
                ApplicationUserId = _currentUser.UserId,
                ContentId = command.ContentId
            }, ct);

            return Result.Success();
        }
    }
}
