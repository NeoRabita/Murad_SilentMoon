using System.Collections.Generic;

namespace SilentMoon.Application.Features.Tracks.Queries.GetTrackMetadata
{
    public class TrackMetadataResponse
    {
        public int Id { get; set; }

        public int ContentId { get; set; }

        public string Title { get; set; }

        public string Duration { get; set; }

        public string AudioUrl { get; set; }

        public List<string> Narrators { get; set; }
    }
}
