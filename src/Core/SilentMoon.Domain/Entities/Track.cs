using SilentMoon.Domain.Common;

namespace SilentMoon.Domain.Entities
{
    public class Track : BaseEntity
    {
        public int ContentId { get; set; }

        public string Title { get; set; }

        public string Duration { get; set; }

        public string AudioUrl { get; set; }

        public int SortOrder { get; set; }
    }
}
