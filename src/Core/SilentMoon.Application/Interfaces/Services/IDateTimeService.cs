using System;

namespace SilentMoon.Application.Interfaces.Services
{
    public interface IDateTimeService
    {
        DateTime NowUtc { get; }
        DateTimeOffset localTime { get; }
    }
}