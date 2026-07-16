using Application.Abstractions.Messaging;
using SilentMoon.Domain.Enums;
using System;

namespace SilentMoon.Application.Features.Reminders.Commands.UpdateReminder
{
    public class UpdateReminderCommand : ICommand<ReminderResponse>
    {
        public int Id { get; set; }

        public TimeSpan? Time { get; set; }

        public DaysOfWeekFlags? Days { get; set; }

        public bool? IsActive { get; set; }
    }
}
