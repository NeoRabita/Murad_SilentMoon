using Application.Abstractions.Messaging;

namespace SilentMoon.Application.Features.Tracks.Queries.GetTrackMetadata
{
    public class GetTrackMetadataQuery : IQuery<TrackMetadataResponse>
    {
        public int TrackId { get; set; }
    }
}
