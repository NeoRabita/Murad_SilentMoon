using Application.Abstractions.Messaging;
using SilentMoon.Application.Common;

namespace SilentMoon.Application.Features.Courses.Queries.GetRelatedCourses
{
    public class GetRelatedCoursesQuery : IQuery<DataEnvelope<CourseSummaryResponse>>
    {
        public int ContentId { get; set; }

        public int? Limit { get; set; }
    }
}
