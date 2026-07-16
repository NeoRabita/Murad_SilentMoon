using SilentMoon.Domain.Enums;
using System;

namespace SilentMoon.Application.Features.Reminders.Commands.UpdateReminder
{
    public class UpdateReminderRequest
    {
        public TimeSpan? Time { get; set; }

        public DaysOfWeekFlags? Days { get; set; }

        public bool? IsActive { get; set; }
    }
}
