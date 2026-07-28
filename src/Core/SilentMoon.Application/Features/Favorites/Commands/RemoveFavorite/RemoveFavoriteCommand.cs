using Application.Abstractions.Messaging;

namespace SilentMoon.Application.Features.Favorites.Commands.RemoveFavorite
{
    public class RemoveFavoriteCommand : ICommand
    {
        public int ContentId { get; set; }
    }
}
