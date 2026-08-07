using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SilentMoon.Application.Features.Tracks.Queries.GetTrackStream;
using System.Threading;
using System.Threading.Tasks;

namespace SilentMoon.WebApi.Controllers
{
    public class TracksController : BaseController
    {
        [HttpGet("{id:int}/stream")]
        public async Task<IResult> Stream(int id, CancellationToken ct)
        {
            var (rangeStart, rangeEnd) = ParseRange(Request.Headers.Range);

            var result = await Dispatcher.Send(new GetTrackStreamQuery
            {
                TrackId = id,
                RangeStart = rangeStart,
                RangeEnd = rangeEnd
            });

            if (!result.IsSuccess)
            {
                return ProblemFactory.CreateProblem(result);
            }

            var track = result.Value;

            Response.Headers.AcceptRanges = "bytes";

            if (rangeStart.HasValue || rangeEnd.HasValue)
            {
                Response.StatusCode = StatusCodes.Status206PartialContent;
                Response.Headers.ContentRange = $"bytes {track.RangeStart}-{track.RangeEnd}/{track.TotalSize}";
            }

            return Results.Stream(track.Content, track.ContentType ?? "audio/mpeg");
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
