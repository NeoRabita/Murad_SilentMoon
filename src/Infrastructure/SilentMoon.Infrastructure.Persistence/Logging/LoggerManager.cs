using NLog;
using SilentMoon.Application.Interfaces.Logging;
using System;

namespace SilentMoon.Infrastructure.Persistence.Logging
{
    public class LoggerManager<T> : IAppLogger<T>
    {
        // NLog-un öz LogManager-indən istifadə edirik
        private readonly ILogger _logger = LogManager.GetLogger(typeof(T).FullName);

        public void LogInformation(string message, params object[] args)
            => _logger.Info(message, args);

        public void LogWarning(string message, params object[] args)
            => _logger.Warn(message, args);

        public void LogError(Exception ex, string message, params object[] args)
            => _logger.Error(ex, message, args);
    }
}
