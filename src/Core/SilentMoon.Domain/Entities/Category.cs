using SilentMoon.Domain.Common;
using SilentMoon.Domain.Enums;

namespace SilentMoon.Domain.Entities
{
    public class Category : BaseEntity
    {
        public string Slug { get; set; }

        public string Title { get; set; }

        public ContentCategory Type { get; set; }

        public string IconUrl { get; set; }

        public int SortOrder { get; set; }
    }
}
