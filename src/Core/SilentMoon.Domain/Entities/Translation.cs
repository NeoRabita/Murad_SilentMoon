using SilentMoon.Domain.Common;

namespace SilentMoon.Domain.Entities
{
    public class Translation : BaseEntity
    {
        public string Key { get; set; }

        public string LanguageCode { get; set; }

        public string Value { get; set; }
    }
}
