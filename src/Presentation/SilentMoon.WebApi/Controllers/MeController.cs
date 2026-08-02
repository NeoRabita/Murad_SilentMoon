using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SilentMoon.Application.Features.User.Commands.UpdateMe;
using SilentMoon.Application.Features.User.Queries.GetHome;
using SilentMoon.Application.Features.User.Queries.GetMe;
using System.Threading.Tasks;

namespace SilentMoon.WebApi.Controllers
{
    public class MeController : BaseController
    {
        [HttpGet]
        public async Task<IResult> GetMe()
        {
            var result = await Dispatcher.Send(new GetMeQuery());

            return HandleResult(result);
        }

        [HttpGet("home")]
        public async Task<IResult> GetHome()
        {
            var result = await Dispatcher.Send(new GetHomeQuery());

            return HandleResult(result);
        }

        [HttpPatch]
        public async Task<IResult> UpdateMe(
            [FromBody] UpdateMeCommand command)
        {
            var result = await Dispatcher.Send(command);

            return HandleResult(result);
        }
    }
}
