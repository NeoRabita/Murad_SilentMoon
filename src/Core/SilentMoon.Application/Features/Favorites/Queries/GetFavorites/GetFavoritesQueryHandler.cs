using Application.Abstractions.Messaging;
using Microsoft.Extensions.Localization;
using SilentMoon.Application.Common.Extensions;
using SilentMoon.Application.Interfaces.Authentication;
using SilentMoon.Application.Interfaces.Services;
using SilentMoon.Domain.Entities;
using SilentMoon.SharedKernel.Resources;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SilentMoon.Application.Features.Favorites.Queries.GetFavorites
{
    public class GetFavoritesQueryHandler : IQueryHandler<GetFavoritesQuery, List<FavoriteItemResponse>>
    {
        private readonly IUow _uow;
        private readonly ICurrentUser _currentUser;
        private readonly IFileStorageService _fileStorage;
        private readonly IStringLocalizer<Messages> _localizer;

        public GetFavoritesQueryHandler(IUow uow,ICurrentUser currentUser,IFileStorageService fileStorage,IStringLocalizer<Messages> localizer)
        {
            _uow = uow;
            _currentUser = currentUser;
            _fileStorage = fileStorage;
            _localizer = localizer;
        }

        public async Task<Result<List<FavoriteItemResponse>>> Handle(GetFavoritesQuery query,CancellationToken ct)
        {
            var favoriteRepo = _uow.GetRepository<Favorite>();

            var favorites = await favoriteRepo.GetAllAsync(ct);

            var myFavorites = favorites.Where(x => x.ApplicationUserId == _currentUser.UserId).ToList();

            var contentRepo = _uow.GetRepository<Content>();

            var contentIds = myFavorites.Select(x => x.ContentId);

            var contents = await contentRepo.GetByIdsAsync(contentIds, ct);

            var contentById = contents.ToDictionary(x => x.Id);

            var items = await Task.WhenAll(myFavorites
                .Where(x => contentById.ContainsKey(x.ContentId))
                .Select(async favorite =>
                {
                    var content = contentById[favorite.ContentId];

                    return new FavoriteItemResponse
                    {
                        ContentId = content.Id,
                        Title = content.Title,
                        Category = _localizer.LocalizeCategory(content.Category),
                        Duration = content.Duration,
                        ThumbnailUrl = await _fileStorage.GetPresignedUrlAsync(MinioBucket.Media, content.ThumbnailUrl, ct)
                    };
                }));

            return items.ToList();
        }
    }
}
