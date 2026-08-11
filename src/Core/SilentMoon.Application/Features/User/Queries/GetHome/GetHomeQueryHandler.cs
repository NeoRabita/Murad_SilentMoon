using Application.Abstractions.Messaging;
using Microsoft.Extensions.Localization;
using SilentMoon.Application.Common.Extensions;
using SilentMoon.Application.Interfaces.Authentication;
using SilentMoon.Application.Interfaces.Services;
using SilentMoon.Domain.Entities;
using SilentMoon.SharedKernel.Resources;
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
        private readonly IDateTimeService _dateTimeService;
        private readonly IFileStorageService _fileStorage;
        private readonly IStringLocalizer<Messages> _localizer;

        public GetHomeQueryHandler(IUow uow,ICurrentUser currentUser,IDateTimeService dateTimeService,IFileStorageService fileStorage,IStringLocalizer<Messages> localizer)
        {
            _uow = uow;
            _currentUser = currentUser;
            _dateTimeService = dateTimeService;
            _fileStorage = fileStorage;
            _localizer = localizer;
        }

        public async Task<Result<HomeResponse>> Handle(GetHomeQuery query,CancellationToken ct)
        {
            var contentRepo = _uow.GetRepository<Content>();

            var contents = await contentRepo.GetAllAsync(ct);

            var dailyThoughtContent = contents
                .Where(x => x.IsDailyThought)
                .OrderBy(x => x.SortOrder)
                .FirstOrDefault();

            var featuredTask = ToResponseListAsync(contents.Where(x => x.IsFeatured).OrderBy(x => x.SortOrder), ct);

            var recommendedTask = ToResponseListAsync(contents.Where(x => x.IsRecommended).OrderBy(x => x.SortOrder), ct);

            var dailyThoughtTask = dailyThoughtContent == null
                ? Task.FromResult<ContentResponse>(null)
                : ToResponseAsync(dailyThoughtContent, ct);

            await Task.WhenAll(featuredTask, recommendedTask, dailyThoughtTask);

            return new HomeResponse
            {
                Greeting = BuildGreeting(_dateTimeService.localTime.Hour),
                UserName = _currentUser.UserName,
                Featured = featuredTask.Result,
                DailyThought = dailyThoughtTask.Result,
                Recommended = recommendedTask.Result
            };
        }

        private string BuildGreeting(int hour)
        {
            var key = hour switch
            {
                >= 5 and < 12 => "Greeting.Morning",
                >= 12 and < 17 => "Greeting.Afternoon",
                >= 17 and < 22 => "Greeting.Evening",
                _ => "Greeting.Night"
            };

            return _localizer[key];
        }

        private async Task<List<ContentResponse>> ToResponseListAsync(
            IEnumerable<Content> contents,
            CancellationToken ct)
        {
            var responses = await Task.WhenAll(contents.Select(content => ToResponseAsync(content, ct)));

            return responses.ToList();
        }
        private async Task<ContentResponse> ToResponseAsync(
            Content content,
            CancellationToken ct) => new()
            {
                Id = content.Id,
                Title = content.Title,
                Category = _localizer.LocalizeCategory(content.Category),
                Duration = content.Duration,
                ThumbnailUrl = await _fileStorage.GetPresignedUrlAsync(MinioBucket.Media, content.ThumbnailUrl, ct)
            };
    }
}
