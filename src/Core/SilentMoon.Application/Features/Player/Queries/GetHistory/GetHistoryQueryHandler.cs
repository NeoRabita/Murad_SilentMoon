using Application.Abstractions.Messaging;
using SilentMoon.Application.Common.Extensions;
using SilentMoon.Application.Common.Pagination;
using SilentMoon.Application.Interfaces.Authentication;
using SilentMoon.Domain.Entities;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SilentMoon.Application.Features.Player.Queries.GetHistory
{
    public class GetHistoryQueryHandler : IQueryHandler<GetHistoryQuery, PagedResponse<HistoryItemResponse>>
    {
        private readonly IUow _uow;
        private readonly ICurrentUser _currentUser;

        public GetHistoryQueryHandler(IUow uow,ICurrentUser currentUser)
        {
            _uow = uow;
            _currentUser = currentUser;
        }

        public async Task<Result<PagedResponse<HistoryItemResponse>>> Handle(GetHistoryQuery query,CancellationToken ct)
        {
            var progressRepo = _uow.GetRepository<PlaybackProgress>();

            var (myProgress, totalCount) = await progressRepo.GetPagedAsync(
                x => x.ApplicationUserId == _currentUser.UserId,
                x => x.UpdatedAtUtc,
                ascending: false,
                page: query.NormalizedPage,
                limit: query.NormalizedLimit,
                cancellationToken: ct);

            var trackRepo = _uow.GetRepository<Track>();

            var tracks = await trackRepo.GetByIdsAsync(myProgress.Select(x => x.TrackId), ct);

            var contentRepo = _uow.GetRepository<Content>();

            var contents = await contentRepo.GetByIdsAsync(tracks.Select(x => x.ContentId), ct);

            var translationRepo = _uow.GetRepository<Translation>();

            var translations = (await translationRepo.GetAllAsync(ct))
                .ToLanguageLookup(CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);

            var items = myProgress
                .Join(tracks, p => p.TrackId, t => t.Id, (progress, track) => new { progress, track })
                .GroupJoin(contents, x => x.track.ContentId, c => c.Id, (x, matchedContents) =>
                {
                    var matchedContent = matchedContents.FirstOrDefault();

                    return new HistoryItemResponse
                    {
                        TrackId = x.track.Id,
                        TrackTitle = translations.Localize(TranslationKeys.For("Track", x.track.Id, "Title"), x.track.Title),
                        ContentId = x.track.ContentId,
                        ContentTitle = matchedContent == null
                            ? null
                            : translations.Localize(TranslationKeys.For("Content", matchedContent.Id, "Title"), matchedContent.Title),
                        PositionSeconds = x.progress.PositionSeconds,
                        UpdatedAtUtc = x.progress.UpdatedAtUtc
                    };
                })
                .ToList();

            return new PagedResponse<HistoryItemResponse>
            {
                Items = items,
                TotalCount = totalCount,
                Page = query.NormalizedPage,
                Limit = query.NormalizedLimit
            };
        }
    }
}
