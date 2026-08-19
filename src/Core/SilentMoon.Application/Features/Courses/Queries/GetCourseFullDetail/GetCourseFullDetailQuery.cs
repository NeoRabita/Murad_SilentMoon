using Application.Abstractions.Messaging;

namespace SilentMoon.Application.Features.Courses.Queries.GetCourseFullDetail
{
    public class GetCourseFullDetailQuery : IQuery<CourseFullDetailResponse>
    {
        public int ContentId { get; set; }
    }
}
