using System;

namespace SilentMoon.Domain.Enums
{
    public static class DayOfWeekExtensions
    {
        public static DaysOfWeekFlags ToDaysOfWeekFlags(this DayOfWeek day) => day switch
        {
            DayOfWeek.Monday => DaysOfWeekFlags.Monday,
            DayOfWeek.Tuesday => DaysOfWeekFlags.Tuesday,
            DayOfWeek.Wednesday => DaysOfWeekFlags.Wednesday,
            DayOfWeek.Thursday => DaysOfWeekFlags.Thursday,
            DayOfWeek.Friday => DaysOfWeekFlags.Friday,
            DayOfWeek.Saturday => DaysOfWeekFlags.Saturday,
            DayOfWeek.Sunday => DaysOfWeekFlags.Sunday,
            _ => DaysOfWeekFlags.None
        };
    }
}
