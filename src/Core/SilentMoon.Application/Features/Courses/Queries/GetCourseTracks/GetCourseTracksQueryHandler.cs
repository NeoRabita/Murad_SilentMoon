using Application.Abstractions.Messaging;
using SilentMoon.Application.Common.Extensions;
using SilentMoon.Domain.Entities;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SilentMoon.Application.Features.Courses.Queries.GetCourseTracks
{
    public class GetCourseTracksQueryHandler : IQueryHandler<GetCourseTracksQuery, List<TrackResponse>>
    {
        private readonly IUow _uow;

        public GetCourseTracksQueryHandler(IUow uow)
        {
            _uow = uow;
        }

        public async Task<Result<List<TrackResponse>>> Handle(GetCourseTracksQuery query, CancellationToken ct)
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

            var tracks = allTracks
                .Where(x => x.ContentId == content.Id)
                .OrderBy(x => x.SortOrder)
                .Select(track => new TrackResponse
                {
                    Id = track.Id,
                    Title = translations.Localize(TranslationKeys.Track(track.Id, "Title"), track.Title),
                    Duration = track.Duration,
                    AudioUrl = $"/api/v1/tracks/{track.Id}/stream"
                })
                .ToList();

            return tracks;
        }
    }
}
