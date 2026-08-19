using SilentMoon.Application.Features.Tracks;
using System.Collections.Generic;

namespace SilentMoon.Application.Features.Courses.Queries.GetCourseFullDetail
{
    public class CourseFullDetailResponse
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Subtitle { get; set; }

        public string Category { get; set; }

        public string ThumbnailUrl { get; set; }

        public List<string> Narrators { get; set; }

        public List<TrackResponse> Tracks { get; set; }
    }
}
