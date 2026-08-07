using Application.Abstractions.Messaging;
using SilentMoon.Application.DTOs.Storage;
using SilentMoon.Application.Interfaces.Services;
using SilentMoon.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace SilentMoon.Application.Features.Tracks.Queries.GetTrackStream
{
    public class GetTrackStreamQueryHandler : IQueryHandler<GetTrackStreamQuery, ObjectRangeResult>
    {
        private readonly IUow _uow;
        private readonly IFileStorageService _fileStorage;

        public GetTrackStreamQueryHandler(IUow uow,IFileStorageService fileStorage)
        {
            _uow = uow;
            _fileStorage = fileStorage;
        }

        public async Task<Result<ObjectRangeResult>> Handle(GetTrackStreamQuery query,CancellationToken ct)
        {
            var trackRepo = _uow.GetRepository<Track>();

            var track = await trackRepo.GetByIdAsync(query.TrackId, ct);

            if (track == null)
            {
                return Error.NotFound(
                    "Track.NotFound",
                    "Track not found");
            }

            return await _fileStorage.GetObjectRangeAsync(
                MinioBucket.Tracks,
                track.AudioUrl,
                query.RangeStart,
                query.RangeEnd,
                ct);
        }
    }
}
