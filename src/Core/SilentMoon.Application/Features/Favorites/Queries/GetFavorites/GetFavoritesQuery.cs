using Application.Abstractions.Messaging;
using System.Collections.Generic;

namespace SilentMoon.Application.Features.Favorites.Queries.GetFavorites
{
    public class GetFavoritesQuery : IQuery<List<FavoriteItemResponse>>
    {
    }
}
