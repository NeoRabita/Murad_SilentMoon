using Application.Abstractions.Messaging;
using SilentMoon.Application.DTOs.Storage;

namespace SilentMoon.Application.Features.Tracks.Queries.GetTrackStream
{
    public class GetTrackStreamQuery : IQuery<ObjectRangeResult>
    {
        public int TrackId { get; set; }

        public long? RangeStart { get; set; }

        public long? RangeEnd { get; set; }
    }
}
