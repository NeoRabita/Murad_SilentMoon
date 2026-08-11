using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SilentMoon.Application.Features.Reminders.Commands.CreateReminder;
using SilentMoon.Application.Features.Reminders.Commands.DeleteReminder;
using SilentMoon.Application.Features.Reminders.Commands.UpdateReminder;
using SilentMoon.Application.Features.Reminders.Queries.GetReminders;
using SilentMoon.Application.Features.User.Commands.SetMyTopics;
using SilentMoon.Application.Features.User.Queries.GetMyTopics;
using System.Threading.Tasks;

namespace SilentMoon.WebApi.Controllers
{
    public class OnboardingController : BaseController
    {
        [HttpGet("topics")]
        public async Task<IResult> GetMyTopics()
        {
            var result = await Dispatcher.Send(new GetMyTopicsQuery());

            return HandleResult(result);
        }

        [HttpPut("topics")]
        public async Task<IResult> SetMyTopics(
            [FromBody] SetMyTopicsCommand command)
        {
            var result = await Dispatcher.Send(command);

            return HandleResult(result);
        }

        [HttpGet("reminders")]
        public async Task<IResult> GetReminders()
        {
            var result = await Dispatcher.Send(new GetRemindersQuery());

            return HandleResult(result);
        }

        [HttpPost("reminders")]
        public async Task<IResult> CreateReminder(
            [FromBody] CreateReminderCommand command)
        {
            var result = await Dispatcher.Send(command);

            return HandleResult(result);
        }

        [HttpPatch("reminders/{id}")]
        public async Task<IResult> UpdateReminder(
            int id,
            [FromBody] UpdateReminderRequest request)
        {
            var result = await Dispatcher.Send(new UpdateReminderCommand
            {
                Id = id,
                Time = request.Time,
                Days = request.Days,
                IsActive = request.IsActive
            });

            return HandleResult(result);
        }

        [HttpDelete("reminders/{id}")]
        public async Task<IResult> DeleteReminder(int id)
        {
            var result = await Dispatcher.Send(new DeleteReminderCommand { Id = id });

            return HandleResult(result);
        }
    }
}
