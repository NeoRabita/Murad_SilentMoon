using SilentMoon.Domain.Common;
using SilentMoon.Domain.Entities.SilentMoon.Domain.Entities;
using System;

namespace SilentMoon.Domain.Entities
{
    public class PlaybackProgress : BaseEntity
    {
        public int ApplicationUserId { get; set; }

        public ApplicationUser ApplicationUser { get; set; }

        public int TrackId { get; set; }

        public Track Track { get; set; }

        public int PositionSeconds { get; set; }

        public DateTime UpdatedAtUtc { get; set; }
    }
}
