using SilentMoon.Domain.Common;
using SilentMoon.Domain.Enums;

namespace SilentMoon.Domain.Entities
{
    public class ContentNarrator : BaseEntity
    {
        public int ContentId { get; set; }

        public Content Content { get; set; }

        public NarratorGender Gender { get; set; }
    }
}
