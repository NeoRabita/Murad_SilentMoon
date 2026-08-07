using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SilentMoon.Application.Interfaces.Repositories;
using SilentMoon.Application.Interfaces.Services;
using SilentMoon.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace SilentMoon.WebApi.Controllers
{
    public class TracksController : BaseController
    {
        private readonly IGenericRepository<Track> _trackRepo;
        private readonly IFileStorageService _fileStorage;

        public TracksController(IGenericRepository<Track> trackRepo,IFileStorageService fileStorage)
        {
            _trackRepo = trackRepo;
            _fileStorage = fileStorage;
        }

        [HttpGet("{id:int}/stream")]
        public async Task<IActionResult> Stream(int id, CancellationToken ct)
        {
            var track = await _trackRepo.GetByIdAsync(id, ct);

            if (track == null)
            {
                return NotFound();
            }

            var (rangeStart, rangeEnd) = ParseRange(Request.Headers.Range);

            var result = await _fileStorage.GetObjectRangeAsync(MinioBucket.Tracks, track.AudioUrl, rangeStart, rangeEnd, ct);

            Response.Headers.AcceptRanges = "bytes";

            if (rangeStart.HasValue || rangeEnd.HasValue)
            {
                Response.StatusCode = StatusCodes.Status206PartialContent;
                Response.Headers.ContentRange = $"bytes {result.RangeStart}-{result.RangeEnd}/{result.TotalSize}";
            }

            return File(result.Content, result.ContentType ?? "audio/mpeg", enableRangeProcessing: false);
        }

        private static (long? Start, long? End) ParseRange(string rangeHeader)
        {
            if (string.IsNullOrEmpty(rangeHeader) || !rangeHeader.StartsWith("bytes="))
            {
                return (null, null);
            }

            var parts = rangeHeader.Substring("bytes=".Length).Split('-');

            long? start = parts.Length > 0 && long.TryParse(parts[0], out var s) ? s : null;
            long? end = parts.Length > 1 && long.TryParse(parts[1], out var e) ? e : null;

            return (start, end);
        }
    }
}
