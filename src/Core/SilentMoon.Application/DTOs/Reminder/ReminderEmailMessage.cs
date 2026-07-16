using System;

namespace SilentMoon.Application.DTOs.Reminder
{
    public class ReminderEmailMessage
    {
        public int UserId { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public TimeSpan Time { get; set; }
    }
}
