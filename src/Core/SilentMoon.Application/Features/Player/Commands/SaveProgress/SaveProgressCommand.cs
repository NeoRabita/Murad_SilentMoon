using Application.Abstractions.Messaging;

namespace SilentMoon.Application.Features.Player.Commands.SaveProgress
{
    public class SaveProgressCommand : ICommand
    {
        public int TrackId { get; set; }

        public int PositionSeconds { get; set; }
    }
}
