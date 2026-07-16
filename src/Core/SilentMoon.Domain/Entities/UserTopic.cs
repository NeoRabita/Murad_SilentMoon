using SilentMoon.Domain.Common;
using SilentMoon.Domain.Entities.SilentMoon.Domain.Entities;

namespace SilentMoon.Domain.Entities
{
    public class UserTopic : BaseEntity
    {
        public int ApplicationUserId { get; set; }

        public ApplicationUser ApplicationUser { get; set; }

        public int TopicId { get; set; }

        public Topic Topic { get; set; }
    }
}
