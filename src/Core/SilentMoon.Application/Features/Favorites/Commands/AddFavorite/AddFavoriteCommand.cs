using Application.Abstractions.Messaging;

namespace SilentMoon.Application.Features.Favorites.Commands.AddFavorite
{
    public class AddFavoriteCommand : ICommand
    {
        public int ContentId { get; set; }
    }
}
