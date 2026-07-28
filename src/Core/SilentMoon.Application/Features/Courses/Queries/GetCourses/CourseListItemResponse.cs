namespace SilentMoon.Application.Features.Courses.Queries.GetCourses
{
    public class CourseListItemResponse
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Category { get; set; }

        public string Duration { get; set; }

        public string ThumbnailUrl { get; set; }
    }
}
