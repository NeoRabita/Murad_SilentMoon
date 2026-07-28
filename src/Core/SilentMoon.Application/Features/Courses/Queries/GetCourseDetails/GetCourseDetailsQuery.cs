using Application.Abstractions.Messaging;

namespace SilentMoon.Application.Features.Courses.Queries.GetCourseDetails
{
    public class GetCourseDetailsQuery : IQuery<CourseDetailsResponse>
    {
        public int ContentId { get; set; }
    }
}
