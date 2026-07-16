using Application.Abstractions.Messaging;
using SilentMoon.Application.Interfaces.Authentication;
using SilentMoon.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace SilentMoon.Application.Features.Reminders.Commands.CreateReminder
{
    public class CreateReminderCommandHandler : ICommandHandler<CreateReminderCommand, ReminderResponse>
    {
        private readonly IUow _uow;
        private readonly ICurrentUser _currentUser;

        public CreateReminderCommandHandler(IUow uow,ICurrentUser currentUser)
        {
            _uow = uow;
            _currentUser = currentUser;
        }

        public async Task<Result<ReminderResponse>> Handle(CreateReminderCommand command,CancellationToken ct)
        {
            var reminderRepo = _uow.GetRepository<Reminder>();

            var reminder = new Reminder
            {
                ApplicationUserId = _currentUser.UserId,
                Time = command.Time,
                Days = command.Days,
                IsActive = command.IsActive
            };

            await reminderRepo.AddAsync(reminder, ct);

            await _uow.SaveChangesAsync(ct);

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
