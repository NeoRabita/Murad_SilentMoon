using Application.Abstractions.Messaging;
using Microsoft.Extensions.Localization;
using SilentMoon.Application.Common.Extensions;
using SilentMoon.Application.Features.Courses.Queries.GetCourses;
using SilentMoon.Application.Interfaces.Services;
using SilentMoon.Domain.Entities;
using SilentMoon.SharedKernel.Resources;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SilentMoon.Application.Features.Courses.Queries.GetRelatedCourses
{
    public class GetRelatedCoursesQueryHandler : IQueryHandler<GetRelatedCoursesQuery, List<CourseListItemResponse>>
    {
        private readonly IUow _uow;
        private readonly IFileStorageService _fileStorage;
        private readonly IStringLocalizer<Messages> _localizer;

        public GetRelatedCoursesQueryHandler(IUow uow, IFileStorageService fileStorage, IStringLocalizer<Messages> localizer)
        {
            _uow = uow;
            _fileStorage = fileStorage;
            _localizer = localizer;
        }

        public async Task<Result<List<CourseListItemResponse>>> Handle(GetRelatedCoursesQuery query, CancellationToken ct)
        {
            var contentRepo = _uow.GetRepository<Content>();

            var content = await contentRepo.GetByIdAsync(query.ContentId, ct);

            if (content == null)
            {
                return Error.NotFound(
                    "Course.NotFound",
                    "Course not found");
            }

            var contentTopicRepo = _uow.GetRepository<ContentTopic>();

            var contentTopics = (await contentTopicRepo.GetAllAsync(ct)).ToList();

            var thisContentTopicIds = contentTopics
                .Where(x => x.ContentId == content.Id)
                .Select(x => x.TopicId)
                .ToHashSet();

            var relatedContentIds = contentTopics
                .Where(x => x.ContentId != content.Id && thisContentTopicIds.Contains(x.TopicId))
                .Select(x => x.ContentId)
                .ToHashSet();

            var allContents = await contentRepo.GetAllAsync(ct);

            var relatedContents = allContents
                .Where(x => relatedContentIds.Contains(x.Id))
                .OrderBy(x => x.SortOrder)
                .ToList();

            var translationRepo = _uow.GetRepository<Translation>();

            var translations = (await translationRepo.GetAllAsync(ct))
                .ToLanguageLookup(CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);

            var items = await Task.WhenAll(relatedContents.Select(async related => new CourseListItemResponse
            {
                Id = related.Id,
                Title = translations.Localize(TranslationKeys.Content(related.Id, "Title"), related.Title),
                Category = _localizer.LocalizeCategory(related.Category),
                Duration = related.Duration,
                ThumbnailUrl = await _fileStorage.GetPresignedUrlAsync(MinioBucket.Media, related.ThumbnailUrl, ct)
            }));

            return items.ToList();
        }
    }
}
