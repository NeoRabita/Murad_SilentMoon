using Application.Abstractions.Messaging;
using SilentMoon.Application.Common;
using SilentMoon.Application.Common.Extensions;
using SilentMoon.Application.Features.Tracks;
using SilentMoon.Domain.Entities;
using SilentMoon.Domain.Enums;
using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SilentMoon.Application.Features.Courses.Queries.GetCourseTracks
{
    public class GetCourseTracksQueryHandler : IQueryHandler<GetCourseTracksQuery, DataEnvelope<TrackResponse>>
    {
        private readonly IUow _uow;

        public GetCourseTracksQueryHandler(IUow uow)
        {
            _uow = uow;
        }

        public async Task<Result<DataEnvelope<TrackResponse>>> Handle(GetCourseTracksQuery query, CancellationToken ct)
        {
            var contentRepo = _uow.GetRepository<Content>();

            var content = await contentRepo.GetByIdAsync(query.ContentId, ct);

            if (content == null)
            {
                return Error.NotFound(
                    "Course.NotFound",
                    "Course not found");
            }

            var trackRepo = _uow.GetRepository<Track>();
            var translationRepo = _uow.GetRepository<Translation>();

            var allTracks = await trackRepo.GetAllAsync(ct);
            var translations = (await translationRepo.GetAllAsync(ct))
                .ToLanguageLookup(CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);

            var tracksQuery = allTracks.Where(x => x.ContentId == content.Id);

            if (!string.IsNullOrWhiteSpace(query.Narrator) &&
                Enum.TryParse<NarratorGender>(query.Narrator, ignoreCase: true, out var narrator))
            {
                tracksQuery = tracksQuery.Where(x => x.Narrator == narrator);
            }

            var tracks = tracksQuery
                .OrderBy(x => x.SortOrder)
                .Select(track => new TrackResponse
                {
                    Id = track.Id,
                    CourseId = track.ContentId,
                    Title = translations.Localize(TranslationKeys.For("Track", track.Id, "Title"), track.Title),
                    Narrator = track.Narrator.ToString().ToLowerInvariant(),
                    DurationSec = track.DurationSeconds,
                    AudioUrl = $"/api/v1/tracks/{track.Id}/stream",
                    MimeType = track.MimeType ?? "audio/mpeg",
                    FileSizeBytes = track.FileSizeBytes,
                    ImageUrl = track.ImageUrl,
                    TrackNumber = track.SortOrder
                })
                .ToList();

            return new DataEnvelope<TrackResponse>(tracks);
        }
    }
}
