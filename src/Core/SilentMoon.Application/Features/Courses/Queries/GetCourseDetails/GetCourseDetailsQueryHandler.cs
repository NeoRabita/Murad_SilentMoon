using Application.Abstractions.Messaging;
using SilentMoon.Application.Interfaces.Services;
using SilentMoon.Domain.Entities;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SilentMoon.Application.Features.Courses.Queries.GetCourseDetails
{
    public class GetCourseDetailsQueryHandler : IQueryHandler<GetCourseDetailsQuery, CourseDetailsResponse>
    {
        private readonly IUow _uow;
        private readonly IFileStorageService _fileStorage;

        public GetCourseDetailsQueryHandler(IUow uow,IFileStorageService fileStorage)
        {
            _uow = uow;
            _fileStorage = fileStorage;
        }

        public async Task<Result<CourseDetailsResponse>> Handle(GetCourseDetailsQuery query,CancellationToken ct)
        {
            var contentRepo = _uow.GetRepository<Content>();

            var content = await contentRepo.GetByIdAsync(query.ContentId, ct);

            if (content == null)
            {
                return Error.NotFound(
                    "Course.NotFound",
                    "Course not found");
            }

            var trackRepo = _uow.GetRepository<Track>();

            var allTracks = await trackRepo.GetAllAsync(ct);

            var tracks = allTracks
                .Where(x => x.ContentId == content.Id)
                .OrderBy(x => x.SortOrder)
                .ToList();

            var trackResponsesTask = Task.WhenAll(tracks.Select(async track => new TrackResponse
            {
                Id = track.Id,
                Title = track.Title,
                Duration = track.Duration,
                AudioUrl = await _fileStorage.GetPresignedUrlAsync(MinioBucket.Tracks, track.AudioUrl, ct)
            }));

            var thumbnailUrlTask = _fileStorage.GetPresignedUrlAsync(MinioBucket.Media, content.ThumbnailUrl, ct);

            await Task.WhenAll(trackResponsesTask, thumbnailUrlTask);

            return new CourseDetailsResponse
            {
                Id = content.Id,
                Title = content.Title,
                Category = content.Category.ToString(),
                ThumbnailUrl = thumbnailUrlTask.Result,
                Tracks = trackResponsesTask.Result.ToList()
            };
        }
    }
}
