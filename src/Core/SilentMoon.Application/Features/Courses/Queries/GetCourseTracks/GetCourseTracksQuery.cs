using Application.Abstractions.Messaging;
using SilentMoon.Application.Common;
using SilentMoon.Application.Features.Tracks;

namespace SilentMoon.Application.Features.Courses.Queries.GetCourseTracks
{
    public class GetCourseTracksQuery : IQuery<DataEnvelope<TrackResponse>>
    {
        public int ContentId { get; set; }

        public string Narrator { get; set; }
    }
}
