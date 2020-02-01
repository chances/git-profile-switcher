using System;
using Microsoft.Extensions.Logging;
#if !DEBUG
using Sentry;
#endif

namespace GitProfileSwitcher.Logs
{
    public class Logging
    {
        private static ILoggerFactory _factory = LoggerFactory.Create(builder => {
            var isDebug = true;
            builder.ClearProviders();
#if !DEBUG
            builder.AddSentry()
            isDebug = false;
#endif
            builder
                .AddFilter("Microsoft", LogLevel.Warning)
                .AddFilter("System", LogLevel.Warning)
                .AddFilter("GitProfileSwitcher.Program", isDebug ? LogLevel.Trace : LogLevel.Error)
                .AddConsole(configure => {
                    configure.LogToStandardErrorThreshold = LogLevel.Critical;
                });
        });
        private static ILogger _logger = _factory.CreateLogger("GitProfileSwitcher.Program");

        public Logging()
        {
        }

        public static void Information(string message, params object[] args) =>
            _logger.LogInformation(message, args);

        public static void Trace(string message, params object[] args) =>
            _logger.LogTrace(message, args);
        public static void Trace(Exception exception, params object[] args) =>
            _logger.LogTrace(exception, exception.Message, args);
        public static void Trace(Exception exception, string message, params object[] args) =>
            _logger.LogTrace(exception, message, args);

        public static void Exception(Exception e, params object[] args) =>
            _logger.LogError(e, e.Message, args);
        public static void Exception(Exception e, string message, params object[] args) =>
            _logger.LogError(e, message, args);
    }
}
