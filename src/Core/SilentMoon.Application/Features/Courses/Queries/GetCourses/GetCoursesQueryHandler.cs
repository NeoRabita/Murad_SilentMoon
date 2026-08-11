using Application.Abstractions.Messaging;
using Microsoft.Extensions.Localization;
using SilentMoon.Application.Common.Extensions;
using SilentMoon.Application.Common.Pagination;
using SilentMoon.Application.Interfaces.Services;
using SilentMoon.Domain.Entities;
using SilentMoon.SharedKernel.Resources;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace SilentMoon.Application.Features.Courses.Queries.GetCourses
{
    public class GetCoursesQueryHandler : IQueryHandler<GetCoursesQuery, PagedResponse<CourseListItemResponse>>
    {
        private readonly IUow _uow;
        private readonly IFileStorageService _fileStorage;
        private readonly IStringLocalizer<Messages> _localizer;

        public GetCoursesQueryHandler(IUow uow,IFileStorageService fileStorage,IStringLocalizer<Messages> localizer)
        {
            _uow = uow;
            _fileStorage = fileStorage;
            _localizer = localizer;
        }

        public async Task<Result<PagedResponse<CourseListItemResponse>>> Handle(GetCoursesQuery query,CancellationToken ct)
        {
            var contentRepo = _uow.GetRepository<Content>();

            Expression<Func<Content, bool>> predicate = null;

            if (query.TopicId.HasValue)
            {
                var contentTopicRepo = _uow.GetRepository<ContentTopic>();

                var contentTopics = await contentTopicRepo.GetAllAsync(ct);

                var contentIds = contentTopics
                    .Where(x => x.TopicId == query.TopicId.Value)
                    .Select(x => x.ContentId)
                    .ToHashSet();

                predicate = x => contentIds.Contains(x.Id);
            }

            var (contents, totalCount) = await contentRepo.GetPagedAsync(
                predicate,
                x => x.SortOrder,
                ascending: true,
                page: query.NormalizedPage,
                limit: query.NormalizedLimit,
                cancellationToken: ct);

            var items = await Task.WhenAll(contents.Select(async content => new CourseListItemResponse
            {
                Id = content.Id,
                Title = content.Title,
                Category = _localizer.LocalizeCategory(content.Category),
                Duration = content.Duration,
                ThumbnailUrl = await _fileStorage.GetPresignedUrlAsync(MinioBucket.Media, content.ThumbnailUrl, ct)
            }));

            return new PagedResponse<CourseListItemResponse>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                Page = query.NormalizedPage,
                Limit = query.NormalizedLimit
            };
        }
    }
}
