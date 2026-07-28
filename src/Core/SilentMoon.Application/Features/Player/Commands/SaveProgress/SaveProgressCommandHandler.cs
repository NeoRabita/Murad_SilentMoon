using Application.Abstractions.Messaging;
using SilentMoon.Application.Interfaces.Authentication;
using SilentMoon.Application.Interfaces.Services;
using SilentMoon.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace SilentMoon.Application.Features.Player.Commands.SaveProgress
{
    public class SaveProgressCommandHandler : ICommandHandler<SaveProgressCommand>
    {
        private readonly IUow _uow;
        private readonly ICurrentUser _currentUser;
        private readonly IDateTimeService _dateTimeService;

        public SaveProgressCommandHandler(IUow uow,ICurrentUser currentUser,IDateTimeService dateTimeService)
        {
            _uow = uow;
            _currentUser = currentUser;
            _dateTimeService = dateTimeService;
        }

        public async Task<Result> Handle(SaveProgressCommand command,CancellationToken ct)
        {
            var trackRepo = _uow.GetRepository<Track>();

            var track = await trackRepo.GetByIdAsync(command.TrackId, ct);

            if (track == null)
            {
                return Error.NotFound(
                    "Track.NotFound",
                    "Track not found");
            }

            var progressRepo = _uow.GetRepository<PlaybackProgress>();

            var progress = await progressRepo.FirstOrDefaultAsync(
                x => x.ApplicationUserId == _currentUser.UserId && x.TrackId == command.TrackId,
                ct);

            if (progress == null)
            {
                await progressRepo.AddAsync(new PlaybackProgress
                {
                    ApplicationUserId = _currentUser.UserId,
                    TrackId = command.TrackId,
                    PositionSeconds = command.PositionSeconds,
                    UpdatedAtUtc = _dateTimeService.NowUtc
                }, ct);
            }
            else
            {
                progress.PositionSeconds = command.PositionSeconds;
                progress.UpdatedAtUtc = _dateTimeService.NowUtc;

                progressRepo.Update(progress);
            }

            return Result.Success();
        }
    }
}
