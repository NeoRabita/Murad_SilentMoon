using Application.Abstractions.Messaging;
using SilentMoon.Application.Common.Extensions;
using SilentMoon.Domain.Entities;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SilentMoon.Application.Features.Tracks.Queries.GetTrackMetadata
{
    public class GetTrackMetadataQueryHandler : IQueryHandler<GetTrackMetadataQuery, TrackMetadataResponse>
    {
        private readonly IUow _uow;

        public GetTrackMetadataQueryHandler(IUow uow)
        {
            _uow = uow;
        }

        public async Task<Result<TrackMetadataResponse>> Handle(GetTrackMetadataQuery query, CancellationToken ct)
        {
            var trackRepo = _uow.GetRepository<Track>();

            var track = await trackRepo.GetByIdAsync(query.TrackId, ct);

            if (track == null)
            {
                return Error.NotFound(
                    "Track.NotFound",
                    "Track not found");
            }

            var translationRepo = _uow.GetRepository<Translation>();
            var contentNarratorRepo = _uow.GetRepository<ContentNarrator>();

            var translations = (await translationRepo.GetAllAsync(ct))
                .ToLanguageLookup(CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);

            var narrators = (await contentNarratorRepo.GetAllAsync(ct))
                .Where(x => x.ContentId == track.ContentId)
                .Select(x => x.Gender.ToString().ToLowerInvariant())
                .ToList();

            return new TrackMetadataResponse
            {
                Id = track.Id,
                ContentId = track.ContentId,
                Title = translations.Localize(TranslationKeys.Track(track.Id, "Title"), track.Title),
                Duration = track.Duration,
                AudioUrl = $"/api/v1/tracks/{track.Id}/stream",
                Narrators = narrators
            };
        }
    }
}
