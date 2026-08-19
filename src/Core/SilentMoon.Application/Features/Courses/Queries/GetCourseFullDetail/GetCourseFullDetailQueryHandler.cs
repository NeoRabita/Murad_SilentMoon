using Application.Abstractions.Messaging;
using Microsoft.Extensions.Localization;
using SilentMoon.Application.Common.Extensions;
using SilentMoon.Application.Interfaces.Services;
using SilentMoon.Domain.Entities;
using SilentMoon.SharedKernel.Resources;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SilentMoon.Application.Features.Courses.Queries.GetCourseFullDetail
{
    public class GetCourseFullDetailQueryHandler : IQueryHandler<GetCourseFullDetailQuery, CourseFullDetailResponse>
    {
        private readonly IUow _uow;
        private readonly IFileStorageService _fileStorage;
        private readonly IStringLocalizer<Messages> _localizer;

        public GetCourseFullDetailQueryHandler(IUow uow, IFileStorageService fileStorage, IStringLocalizer<Messages> localizer)
        {
            _uow = uow;
            _fileStorage = fileStorage;
            _localizer = localizer;
        }

        public async Task<Result<CourseFullDetailResponse>> Handle(GetCourseFullDetailQuery query, CancellationToken ct)
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
            var contentNarratorRepo = _uow.GetRepository<ContentNarrator>();

            var allTracks = await trackRepo.GetAllAsync(ct);
            var translations = (await translationRepo.GetAllAsync(ct))
                .ToLanguageLookup(CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
            var narrators = (await contentNarratorRepo.GetAllAsync(ct))
                .Where(x => x.ContentId == content.Id)
                .Select(x => x.Gender.ToString().ToLowerInvariant())
                .ToList();

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

            return new CourseFullDetailResponse
            {
                Id = content.Id,
                Title = translations.Localize(TranslationKeys.Content(content.Id, "Title"), content.Title),
                Subtitle = translations.Localize(TranslationKeys.Content(content.Id, "Subtitle"), content.Subtitle),
                Category = _localizer.LocalizeCategory(content.Category),
                ThumbnailUrl = await _fileStorage.GetPresignedUrlAsync(MinioBucket.Media, content.ThumbnailUrl, ct),
                Narrators = narrators,
                Tracks = tracks
            };
        }
    }
}
