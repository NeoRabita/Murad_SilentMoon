using Application.Abstractions.Messaging;
using System.Collections.Generic;

namespace SilentMoon.Application.Features.Courses.Queries.GetCourseTracks
{
    public class GetCourseTracksQuery : IQuery<List<TrackResponse>>
    {
        public int ContentId { get; set; }
    }
}
