using SilentMoon.Domain.Common;
using SilentMoon.Domain.Enums;

namespace SilentMoon.Domain.Entities
{
    public class Content : BaseEntity
    {
        public string Title { get; set; }

        public string Subtitle { get; set; }

        public ContentCategory Category { get; set; }

        public string Duration { get; set; }

        public int DurationSeconds { get; set; }

        public string ThumbnailUrl { get; set; }

        public bool IsFeatured { get; set; }

        public bool IsDailyThought { get; set; }

        public bool IsRecommended { get; set; }

        public bool IsPopular { get; set; }

        public int SortOrder { get; set; }
    }
}
