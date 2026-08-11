using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SilentMoon.Application.Features.Favorites.Commands.AddFavorite;
using SilentMoon.Application.Features.Favorites.Commands.RemoveFavorite;
using SilentMoon.Application.Features.Favorites.Queries.GetFavorites;
using System.Threading.Tasks;

namespace SilentMoon.WebApi.Controllers
{
    public class FavoritesController : BaseController
    {
        [HttpGet]
        public async Task<IResult> GetFavorites()
        {
            var result = await Dispatcher.Send(new GetFavoritesQuery());

            return HandleResult(result);
        }

        [HttpPost]
        public async Task<IResult> AddFavorite(
            [FromBody] AddFavoriteCommand command)
        {
            var result = await Dispatcher.Send(command);

            return HandleResult(result);
        }

        [HttpDelete("{id:int}")]
        public async Task<IResult> RemoveFavorite(int id)
        {
            var result = await Dispatcher.Send(new RemoveFavoriteCommand { ContentId = id });

            return HandleResult(result);
        }
    }
}
