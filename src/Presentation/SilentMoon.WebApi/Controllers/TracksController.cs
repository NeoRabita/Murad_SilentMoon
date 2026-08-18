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
        public async Task<IActionResult> Stream(int id, CancellationToken ct)
        {
            var result = await Dispatcher.Send(new GetTrackStreamQuery
            {
                TrackId = id,
                RangeHeader = Request.Headers.Range
            });

            return HandleStreamResult(result);
        }
    }
}
