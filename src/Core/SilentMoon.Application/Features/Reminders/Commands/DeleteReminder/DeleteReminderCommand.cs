using Application.Abstractions.Messaging;

namespace SilentMoon.Application.Features.Reminders.Commands.DeleteReminder
{
    public class DeleteReminderCommand : ICommand
    {
        public int Id { get; set; }
    }
}
