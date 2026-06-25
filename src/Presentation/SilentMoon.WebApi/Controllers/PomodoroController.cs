using System.Threading.Tasks;
using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SilentMoon.Application.Features.Pomodoros.Commands.CreatePomodoro;
using SilentMoon.Application.Features.Pomodoros.Queries.GetPomodoroColors;

namespace SilentMoon.WebApi.Controllers
{
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    public class PomodoroController : BaseController
    {
        [HttpPost]
        public async Task<IResult> Create([FromBody] CreatePomodoroCommand command)
        {
            var result = await Dispatcher.Send(command);
            return HandleResult(result);
        }

        [MapToApiVersion("1.0")]
        [HttpGet("pomodoro-colors")]
        public async Task<IResult> PomodoroColors()
        {
            var result = await Dispatcher.Send(new GetPomodoroColorsQuery());
            return HandleResult(result);
        }

        [MapToApiVersion("2.0")]
        [HttpGet("pomodoro-colors")]
        public async Task<IResult> PomodoroColorsV2()
        {
            var result = await Dispatcher.Send(new GetPomodoroColorsQuery());
            return HandleResult(result);
        }
    }
}