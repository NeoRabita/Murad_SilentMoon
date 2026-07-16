using Application.Abstractions.Messaging;
using SilentMoon.Application.Interfaces.Authentication;
using SilentMoon.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace SilentMoon.Application.Features.Reminders.Commands.UpdateReminder
{
    public class UpdateReminderCommandHandler : ICommandHandler<UpdateReminderCommand, ReminderResponse>
    {
        private readonly IUow _uow;
        private readonly ICurrentUser _currentUser;

        public UpdateReminderCommandHandler(IUow uow,ICurrentUser currentUser)
        {
            _uow = uow;
            _currentUser = currentUser;
        }

        public async Task<Result<ReminderResponse>> Handle(UpdateReminderCommand command,CancellationToken ct)
        {
            var reminderRepo = _uow.GetRepository<Reminder>();

            var reminder = await reminderRepo.FirstOrDefaultAsync(
                x => x.Id == command.Id && x.ApplicationUserId == _currentUser.UserId,
                ct);

            if (reminder == null)
            {
                return Error.NotFound(
                    "Reminder.NotFound",
                    "Reminder not found");
            }

            if (command.Time.HasValue)
            {
                reminder.Time = command.Time.Value;
            }

            if (command.Days.HasValue)
            {
                reminder.Days = command.Days.Value;
            }

            if (command.IsActive.HasValue)
            {
                reminder.IsActive = command.IsActive.Value;
            }

            reminderRepo.Update(reminder);

            return new ReminderResponse
            {
                Id = reminder.Id,
                Time = reminder.Time,
                Days = reminder.Days,
                IsActive = reminder.IsActive
            };
        }
    }
}
