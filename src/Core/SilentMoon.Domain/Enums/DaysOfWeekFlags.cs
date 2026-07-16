using System;

namespace SilentMoon.Domain.Enums
{
    [Flags]
    public enum DaysOfWeekFlags
    {
        None = 0,
        Monday = 1,
        Tuesday = 2,
        Wednesday = 4,
        Thursday = 8,
        Friday = 16,
        Saturday = 32,
        Sunday = 64
    }
}
