using System.Collections.Generic;

namespace SilentMoon.Application.Features.Courses
{
    public class CourseSummaryResponse
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Subtitle { get; set; }

        public string Type { get; set; }

        public string CategoryId { get; set; }

        public string ImageUrl { get; set; }

        public int DurationSec { get; set; }

        public bool IsFeatured { get; set; }

        public List<string> Narrators { get; set; }
    }
}
