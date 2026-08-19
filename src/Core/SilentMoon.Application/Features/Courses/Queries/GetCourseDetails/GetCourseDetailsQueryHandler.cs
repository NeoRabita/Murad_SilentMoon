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

            var translationRepo = _uow.GetRepository<Translation>();
            var contentNarratorRepo = _uow.GetRepository<ContentNarrator>();

            var translations = (await translationRepo.GetAllAsync(ct))
                .ToLanguageLookup(CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);

            var narrators = (await contentNarratorRepo.GetAllAsync(ct))
                .Where(x => x.ContentId == content.Id)
                .Select(x => x.Gender.ToString().ToLowerInvariant())
                .ToList();

            return new CourseDetailsResponse
            {
                Id = content.Id,
                Title = translations.Localize(TranslationKeys.For("Content", content.Id, "Title"), content.Title),
                Subtitle = translations.Localize(TranslationKeys.For("Content", content.Id, "Subtitle"), content.Subtitle),
                Category = _localizer.LocalizeCategory(content.Category),
                ThumbnailUrl = await _fileStorage.GetPresignedUrlAsync(MinioBucket.Media, content.ThumbnailUrl, ct),
                Narrators = narrators
            };
        }
    }
}
