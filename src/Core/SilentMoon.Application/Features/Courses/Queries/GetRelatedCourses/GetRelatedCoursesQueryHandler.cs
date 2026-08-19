using Application.Abstractions.Messaging;
using SilentMoon.Application.Common;
using SilentMoon.Application.Common.Extensions;
using SilentMoon.Application.Interfaces.Services;
using SilentMoon.Domain.Entities;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SilentMoon.Application.Features.Courses.Queries.GetRelatedCourses
{
    public class GetRelatedCoursesQueryHandler : IQueryHandler<GetRelatedCoursesQuery, DataEnvelope<CourseSummaryResponse>>
    {
        private readonly IUow _uow;
        private readonly IFileStorageService _fileStorage;

        public GetRelatedCoursesQueryHandler(IUow uow, IFileStorageService fileStorage)
        {
            _uow = uow;
            _fileStorage = fileStorage;
        }

        public async Task<Result<DataEnvelope<CourseSummaryResponse>>> Handle(GetRelatedCoursesQuery query, CancellationToken ct)
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
            var contentNarratorRepo = _uow.GetRepository<ContentNarrator>();
            var translationRepo = _uow.GetRepository<Translation>();

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
                .AsEnumerable();

            if (query.Limit.HasValue)
            {
                relatedContents = relatedContents.Take(query.Limit.Value);
            }

            var categoryIdByContentId = contentTopics
                .GroupBy(x => x.ContentId)
                .ToDictionary(g => g.Key, g => g.First().TopicId);

            var narratorsByContentId = (await contentNarratorRepo.GetAllAsync(ct))
                .GroupBy(x => x.ContentId)
                .ToDictionary(g => g.Key, g => g.Select(n => n.Gender.ToString().ToLowerInvariant()).ToList());

            var translations = (await translationRepo.GetAllAsync(ct))
                .ToLanguageLookup(CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);

            var items = await Task.WhenAll(relatedContents.Select(async related => new CourseSummaryResponse
            {
                Id = related.Id,
                Title = translations.Localize(TranslationKeys.For("Content", related.Id, "Title"), related.Title),
                Subtitle = translations.Localize(TranslationKeys.For("Content", related.Id, "Subtitle"), related.Subtitle),
                Type = related.Category.ToString().ToLowerInvariant(),
                CategoryId = categoryIdByContentId.TryGetValue(related.Id, out var categoryId) ? categoryId : null,
                ImageUrl = await _fileStorage.GetPresignedUrlAsync(MinioBucket.Media, related.ThumbnailUrl, ct),
                DurationSec = related.DurationSeconds,
                IsFeatured = related.IsFeatured,
                Narrators = narratorsByContentId.GetValueOrDefault(related.Id) ?? new List<string>()
            }));

            return new DataEnvelope<CourseSummaryResponse>(items.ToList());
        }
    }
}
