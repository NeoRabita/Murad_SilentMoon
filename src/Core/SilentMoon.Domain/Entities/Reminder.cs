using SilentMoon.Domain.Common;
using SilentMoon.Domain.Entities.SilentMoon.Domain.Entities;
using SilentMoon.Domain.Enums;
using System;

namespace SilentMoon.Domain.Entities
{
    public class Reminder : BaseEntity
    {
        public int ApplicationUserId { get; set; }

        public ApplicationUser ApplicationUser { get; set; }

        public TimeSpan Time { get; set; }

        public DaysOfWeekFlags Days { get; set; }

        public bool IsActive { get; set; }
    }
}
