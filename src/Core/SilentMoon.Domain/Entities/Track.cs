using SilentMoon.Domain.Common;
using SilentMoon.Domain.Enums;

namespace SilentMoon.Domain.Entities
{
    public class Track : BaseEntity
    {
        public int ContentId { get; set; }

        public string Title { get; set; }

        public string Duration { get; set; }

        public int DurationSeconds { get; set; }

        public string AudioUrl { get; set; }

        public NarratorGender Narrator { get; set; }

        public string MimeType { get; set; }

        public long? FileSizeBytes { get; set; }

        public string ImageUrl { get; set; }

        public int SortOrder { get; set; }
    }
}
