using System.IO;

namespace SilentMoon.Application.DTOs.Storage
{
    public class ObjectRangeResult
    {
        public Stream Content { get; set; }

        public long TotalSize { get; set; }

        public long RangeStart { get; set; }

        public long RangeEnd { get; set; }

        public string ContentType { get; set; }
    }
}
