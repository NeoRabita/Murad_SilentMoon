using System;
using SilentMoon.Application.Interfaces.Services;

namespace SilentMoon.Infrastructure.Persistence.Services
{
    public class DateTimeService : IDateTimeService
    {
        public DateTimeOffset localTime => TimeZoneInfo.ConvertTime(DateTimeOffset.Now, TimeZoneInfo.FindSystemTimeZoneById("Azerbaijan Standard Time"));
        public DateTime NowUtc => localTime.DateTime;
    }
}