using System.Collections.Generic;

namespace SilentMoon.Application.Features.Courses.Queries.GetCourseDetails
{
    public class CourseDetailsResponse
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Category { get; set; }

        public string ThumbnailUrl { get; set; }

        public List<TrackResponse> Tracks { get; set; }
    }
}
