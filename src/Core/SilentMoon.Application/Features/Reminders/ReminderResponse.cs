using SilentMoon.Domain.Enums;
using System;

namespace SilentMoon.Application.Features.Reminders
{
    public class ReminderResponse
    {
        public int Id { get; set; }

        public TimeSpan Time { get; set; }

        public DaysOfWeekFlags Days { get; set; }

        public bool IsActive { get; set; }
    }
}
