using Application.Abstractions.Messaging;

namespace SilentMoon.Application.Features.Player.Queries.GetProgress
{
    public class GetProgressQuery : IQuery<ProgressResponse>
    {
        public int TrackId { get; set; }
    }
}
