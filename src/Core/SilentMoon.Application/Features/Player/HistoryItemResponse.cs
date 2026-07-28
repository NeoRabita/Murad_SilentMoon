using System;

namespace SilentMoon.Application.Features.Player
{
    public class HistoryItemResponse
    {
        public int TrackId { get; set; }

        public string TrackTitle { get; set; }

        public int ContentId { get; set; }

        public string ContentTitle { get; set; }

        public int PositionSeconds { get; set; }

        public DateTime UpdatedAtUtc { get; set; }
    }
}
