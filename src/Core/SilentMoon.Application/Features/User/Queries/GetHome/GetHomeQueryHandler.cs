using Application.Abstractions.Messaging;
using SilentMoon.Application.Interfaces.Authentication;
using SilentMoon.Application.Interfaces.Services;
using SilentMoon.Domain.Entities;
using SilentMoon.Domain.Enums;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SilentMoon.Application.Features.User.Queries.GetHome
{
    public class GetHomeQueryHandler : IQueryHandler<GetHomeQuery, HomeResponse>
    {
        private readonly IUow _uow;
        private readonly ICurrentUser _currentUser;
        private readonly IFileStorageService _fileStorage;

        public GetHomeQueryHandler(IUow uow, ICurrentUser currentUser, IFileStorageService fileStorage)
        {
            _uow = uow;
            _currentUser = currentUser;
            _fileStorage = fileStorage;
        }

        public async Task<Result<HomeResponse>> Handle(GetHomeQuery query, CancellationToken ct)
        {
            var contentRepo = _uow.GetRepository<Content>();
            var contentTopicRepo = _uow.GetRepository<ContentTopic>();
            var userTopicRepo = _uow.GetRepository<UserTopic>();
            var contentNarratorRepo = _uow.GetRepository<ContentNarrator>();

            var contents = (await contentRepo.GetAllAsync(ct)).ToList();
            var contentTopics = (await contentTopicRepo.GetAllAsync(ct)).ToList();
            var userTopics = (await userTopicRepo.GetAllAsync(ct)).ToList();
            var contentNarrators = (await contentNarratorRepo.GetAllAsync(ct)).ToList();

            var categoryIdByContentId = contentTopics
                .GroupBy(x => x.ContentId)
                .ToDictionary(g => g.Key, g => g.First().TopicId.ToString());

            var narratorsByContentId = contentNarrators
                .GroupBy(x => x.ContentId)
                .ToDictionary(g => g.Key, g => g.Select(n => n.Gender.ToString().ToLowerInvariant()).ToList());

            var recommendedTopicIds = userTopics
                .Where(x => x.ApplicationUserId == _currentUser.UserId)
                .Select(x => x.TopicId)
                .ToHashSet();

            var recommendedContentIds = contentTopics
                .Where(x => recommendedTopicIds.Contains(x.TopicId))
                .Select(x => x.ContentId)
                .ToHashSet();

            var recommendedContents = contents
                .Where(x => recommendedContentIds.Contains(x.Id))
                .OrderBy(x => x.SortOrder);

            var dailyThoughtContent = contents
                .Where(x => x.IsDailyThought)
                .OrderBy(x => x.SortOrder)
                .FirstOrDefault();

            var featuredSleepContents = contents
                .Where(x => x.Category == ContentCategory.Sleep && x.IsFeatured)
                .OrderBy(x => x.SortOrder);

            var popularMeditationContents = contents
                .Where(x => x.Category == ContentCategory.Meditation && x.IsPopular)
                .OrderBy(x => x.SortOrder);

            var recommendedTask = ToItemListAsync(recommendedContents, categoryIdByContentId, narratorsByContentId, ct);
            var featuredSleepTask = ToItemListAsync(featuredSleepContents, categoryIdByContentId, narratorsByContentId, ct);
            var popularMeditationsTask = ToItemListAsync(popularMeditationContents, categoryIdByContentId, narratorsByContentId, ct);
            var dailyThoughtTask = dailyThoughtContent == null
                ? Task.FromResult<HomeItemResponse>(null)
                : ToItemAsync(dailyThoughtContent, categoryIdByContentId, narratorsByContentId, ct);

            await Task.WhenAll(recommendedTask, featuredSleepTask, popularMeditationsTask, dailyThoughtTask);

            return new HomeResponse
            {
                Recommended = new HomeSectionResponse { Title = "Recommended For You", Items = recommendedTask.Result },
                DailyThought = dailyThoughtTask.Result,
                FeaturedSleep = new HomeSectionResponse { Title = "Sleep", Items = featuredSleepTask.Result },
                PopularMeditations = new HomeSectionResponse { Title = "Popular Meditations", Items = popularMeditationsTask.Result }
            };
        }

        private async Task<List<HomeItemResponse>> ToItemListAsync(
            IEnumerable<Content> contents,
            Dictionary<int, string> categoryIdByContentId,
            Dictionary<int, List<string>> narratorsByContentId,
            CancellationToken ct)
        {
            var responses = await Task.WhenAll(contents.Select(content =>
                ToItemAsync(content, categoryIdByContentId, narratorsByContentId, ct)));

            return responses.ToList();
        }

        private async Task<HomeItemResponse> ToItemAsync(
            Content content,
            Dictionary<int, string> categoryIdByContentId,
            Dictionary<int, List<string>> narratorsByContentId,
            CancellationToken ct) => new()
            {
                Id = $"course_{content.Id}",
                Title = content.Title,
                Subtitle = content.Subtitle,
                Type = content.Category.ToString().ToLowerInvariant(),
                CategoryId = categoryIdByContentId.GetValueOrDefault(content.Id),
                ImageUrl = await _fileStorage.GetPresignedUrlAsync(MinioBucket.Media, content.ThumbnailUrl, ct),
                DurationSec = content.DurationSeconds,
                IsFeatured = content.IsFeatured,
                Narrators = narratorsByContentId.GetValueOrDefault(content.Id) ?? new List<string>()
            };
    }
}
