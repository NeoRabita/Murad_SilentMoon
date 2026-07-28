using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SilentMoon.Application.Features.Player.Commands.SaveProgress;
using SilentMoon.Application.Features.Player.Queries.GetHistory;
using SilentMoon.Application.Features.Player.Queries.GetProgress;
using System.Threading.Tasks;

namespace SilentMoon.WebApi.Controllers
{
    [Authorize]
    public class PlayerController : BaseController
    {
        [HttpPost("progress")]
        public async Task<IResult> SaveProgress(
            [FromBody] SaveProgressCommand command)
        {
            var result = await Dispatcher.Send(command);

            return HandleResult(result);
        }

        [HttpGet("progress/{trackId:int}")]
        public async Task<IResult> GetProgress(int trackId)
        {
            var result = await Dispatcher.Send(new GetProgressQuery
            {
                TrackId = trackId
            });

            return HandleResult(result);
        }

        [HttpGet("history")]
        public async Task<IResult> GetHistory([FromQuery] GetHistoryQuery query)
        {
            var result = await Dispatcher.Send(query);

            return HandleResult(result);
        }
    }
}
