using Application.Abstractions.Messaging;
using SilentMoon.Application.Common.Pagination;

namespace SilentMoon.Application.Features.Courses.Queries.GetCourses
{
    public class GetCoursesQuery : PagedQuery, IQuery<PagedResponse<CourseListItemResponse>>
    {
        public int? TopicId { get; set; }
    }
}
