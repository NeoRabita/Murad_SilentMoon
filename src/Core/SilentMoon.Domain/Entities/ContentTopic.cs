using SilentMoon.Domain.Common;

namespace SilentMoon.Domain.Entities
{
    public class ContentTopic : BaseEntity
    {
        public int ContentId { get; set; }

        public Content Content { get; set; }

        public int TopicId { get; set; }

        public Topic Topic { get; set; }
    }
}
