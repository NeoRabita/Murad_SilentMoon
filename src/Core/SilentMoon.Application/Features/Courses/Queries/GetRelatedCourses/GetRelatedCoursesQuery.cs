using Application.Abstractions.Messaging;
using SilentMoon.Application.Features.Courses.Queries.GetCourses;
using System.Collections.Generic;

namespace SilentMoon.Application.Features.Courses.Queries.GetRelatedCourses
{
    public class GetRelatedCoursesQuery : IQuery<List<CourseListItemResponse>>
    {
        public int ContentId { get; set; }
    }
}
