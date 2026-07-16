using Application.Abstractions.Messaging;
using SilentMoon.Domain.Enums;
using System;

namespace SilentMoon.Application.Features.Reminders.Commands.CreateReminder
{
    public class CreateReminderCommand : ICommand<ReminderResponse>
    {
        public TimeSpan Time { get; set; }

        public DaysOfWeekFlags Days { get; set; }

        public bool IsActive { get; set; }
    }
}
