using SilentMoon.Domain.Common;
using SilentMoon.Domain.Entities.SilentMoon.Domain.Entities;

namespace SilentMoon.Domain.Entities
{
    public class Favorite : BaseEntity
    {
        public int ApplicationUserId { get; set; }

        public ApplicationUser ApplicationUser { get; set; }

        public int ContentId { get; set; }

        public Content Content { get; set; }
    }
}
