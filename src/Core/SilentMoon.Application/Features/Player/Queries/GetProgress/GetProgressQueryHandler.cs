using Application.Abstractions.Messaging;
using SilentMoon.Application.Interfaces.Authentication;
using SilentMoon.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace SilentMoon.Application.Features.Player.Queries.GetProgress
{
    public class GetProgressQueryHandler : IQueryHandler<GetProgressQuery, ProgressResponse>
    {
        private readonly IUow _uow;
        private readonly ICurrentUser _currentUser;

        public GetProgressQueryHandler(IUow uow,ICurrentUser currentUser)
        {
            _uow = uow;
            _currentUser = currentUser;
        }

        public async Task<Result<ProgressResponse>> Handle(GetProgressQuery query,CancellationToken ct)
        {
            var progressRepo = _uow.GetRepository<PlaybackProgress>();

            var progress = await progressRepo.FirstOrDefaultAsync(
                x => x.ApplicationUserId == _currentUser.UserId && x.TrackId == query.TrackId,
                ct);

            return new ProgressResponse
            {
                TrackId = query.TrackId,
                PositionSeconds = progress?.PositionSeconds ?? 0
            };
        }
    }
}
