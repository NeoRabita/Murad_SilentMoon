using Application.Abstractions.Messaging;
using SilentMoon.Application.Interfaces.Authentication;
using SilentMoon.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace SilentMoon.Application.Features.Reminders.Commands.DeleteReminder
{
    public class DeleteReminderCommandHandler : ICommandHandler<DeleteReminderCommand>
    {
        private readonly IUow _uow;
        private readonly ICurrentUser _currentUser;

        public DeleteReminderCommandHandler(IUow uow,ICurrentUser currentUser)
        {
            _uow = uow;
            _currentUser = currentUser;
        }

        public async Task<Result> Handle(DeleteReminderCommand command,CancellationToken ct)
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

            reminderRepo.Delete(reminder);

            return Result.Success();
        }
    }
}
