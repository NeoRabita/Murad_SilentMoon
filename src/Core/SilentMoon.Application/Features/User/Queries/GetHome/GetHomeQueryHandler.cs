using Application.Abstractions.Messaging;
using SilentMoon.Application.Interfaces.Authentication;
using SilentMoon.Application.Interfaces.Services;
using SilentMoon.Domain.Entities;
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

        public GetHomeQueryHandler(IUow uow,ICurrentUser currentUser,IDateTimeService dateTimeService,IFileStorageService fileStorage)
        {
            _uow = uow;
            _currentUser = currentUser;
            _dateTimeService = dateTimeService;
            _fileStorage = fileStorage;
        }

        public async Task<Result<HomeResponse>> Handle(GetHomeQuery query,CancellationToken ct)
        {
            var contentRepo = _uow.GetRepository<Content>();

            var contents = await contentRepo.GetAllAsync(ct);

            var featured = await ToResponseListAsync(contents.Where(x => x.IsFeatured).OrderBy(x => x.SortOrder),ct);

            var recommended = await ToResponseListAsync(contents.Where(x => x.IsRecommended).OrderBy(x => x.SortOrder),ct);

            var dailyThoughtContent = contents
                .Where(x => x.IsDailyThought)
                .OrderBy(x => x.SortOrder)
                .FirstOrDefault();

            var dailyThought = dailyThoughtContent == null? null: await ToResponseAsync(dailyThoughtContent, ct);

            return new HomeResponse
            {
                Greeting = BuildGreeting(_dateTimeService.localTime.Hour),
                UserName = _currentUser.UserName,
                Featured = featured,
                DailyThought = dailyThought,
                Recommended = recommended
            };
        }

        private static string BuildGreeting(int hour) => hour switch
        {
            >= 5 and < 12 => "Good Morning",
            >= 12 and < 17 => "Good Afternoon",
            >= 17 and < 22 => "Good Evening",
            _ => "Good Night"
        };

        private async Task<List<ContentResponse>> ToResponseListAsync(
            IEnumerable<Content> contents,
            CancellationToken ct)
        {
            var responses = new List<ContentResponse>();

            foreach (var content in contents)
            {
                responses.Add(await ToResponseAsync(content, ct));
            }

            return responses;
        }

        private async Task<ContentResponse> ToResponseAsync(
            Content content,
            CancellationToken ct) => new()
            {
                Id = content.Id,
                Title = content.Title,
                Category = content.Category.ToString(),
                Duration = content.Duration,
                ThumbnailUrl = await _fileStorage.GetPresignedUrlAsync(content.ThumbnailUrl, ct)
            };
    }
}
