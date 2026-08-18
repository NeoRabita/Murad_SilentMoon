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

namespace SilentMoon.Application.Features.Courses.Queries.GetCourseDetails
{
    public class GetCourseDetailsQueryHandler : IQueryHandler<GetCourseDetailsQuery, CourseDetailsResponse>
    {
        private readonly IUow _uow;
        private readonly IFileStorageService _fileStorage;
        private readonly IStringLocalizer<Messages> _localizer;

        public GetCourseDetailsQueryHandler(IUow uow,IFileStorageService fileStorage,IStringLocalizer<Messages> localizer)
        {
            _uow = uow;
            _fileStorage = fileStorage;
            _localizer = localizer;
        }

        public async Task<Result<CourseDetailsResponse>> Handle(GetCourseDetailsQuery query,CancellationToken ct)
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

            var allTracks = await trackRepo.GetAllAsync(ct);

            var tracks = allTracks
                .Where(x => x.ContentId == content.Id)
                .OrderBy(x => x.SortOrder)
                .ToList();

            var translationRepo = _uow.GetRepository<Translation>();

            var translations = (await translationRepo.GetAllAsync(ct))
                .ToLanguageLookup(CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);

            var trackResponsesTask = Task.WhenAll(tracks.Select(async track => new TrackResponse
            {
                Id = track.Id,
                Title = translations.Localize(TranslationKeys.Track(track.Id, "Title"), track.Title),
                Duration = track.Duration,
                AudioUrl = await _fileStorage.GetPresignedUrlAsync(MinioBucket.Tracks, track.AudioUrl, ct)
            }));

            var thumbnailUrlTask = _fileStorage.GetPresignedUrlAsync(MinioBucket.Media, content.ThumbnailUrl, ct);

            await Task.WhenAll(trackResponsesTask, thumbnailUrlTask);

            return new CourseDetailsResponse
            {
                Id = content.Id,
                Title = translations.Localize(TranslationKeys.Content(content.Id, "Title"), content.Title),
                Category = _localizer.LocalizeCategory(content.Category),
                ThumbnailUrl = thumbnailUrlTask.Result,
                Tracks = trackResponsesTask.Result.ToList()
            };
        }
    }
}
