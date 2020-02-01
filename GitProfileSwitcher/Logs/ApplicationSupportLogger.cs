using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Text;
using Foundation;
using Microsoft.Extensions.Logging;

namespace GitProfileSwitcher.Logs
{
    public class ApplicationSupportLogger : ILogger, IDisposable
    {
        private static readonly string _libraryDir = NSSearchPath.GetDirectories(
                NSSearchPathDirectory.LibraryDirectory, NSSearchPathDomain.User)
                .FirstOrDefault();
        private readonly FileStream _fileStream;

        public ApplicationSupportLogger(string name = null)
        {
            var logDirectory = Path.Combine(
                _libraryDir, "Logs", "GitProfileSwitcher");
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }

            var logFileName = Path.Combine(logDirectory,
                $"{NSBundle.MainBundle.BundleIdentifier}-{name}.log");
            _fileStream = new FileStream(logFileName, FileMode.Append, FileAccess.Write);
        }

        ~ApplicationSupportLogger()
        {
            Dispose();
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public void Dispose()
        {
            _fileStream.Flush();
            _fileStream.Dispose();
        }

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            using var writer = new StreamWriter(_fileStream, Encoding.UTF8);
            var levelEmojiOrFallback = logLevel.ToString();
            switch (logLevel)
            {
                case LogLevel.Trace:
                case LogLevel.Debug:
                    levelEmojiOrFallback = $"❓ ({logLevel.ToString().ToLowerInvariant()})";
                    break;
                case LogLevel.Information:
                    levelEmojiOrFallback = "ℹ️";
                    break;
                case LogLevel.Warning:
                    levelEmojiOrFallback = "⚠️";
                    break;
                case LogLevel.Error:
                    levelEmojiOrFallback = "❗️";
                    break;
                case LogLevel.Critical:
                    levelEmojiOrFallback = "‼️";
                    break;
                default:
                    break;
            }
            writer.WriteLine($"{levelEmojiOrFallback}: {formatter(state, exception)}");
            _fileStream.Flush();
            writer.Dispose();
        }
    }

    public class ApplicationSupportLoggerProvider : ILoggerProvider
    {
        private readonly ConcurrentDictionary<string, ApplicationSupportLogger> _loggers =
            new ConcurrentDictionary<string, ApplicationSupportLogger>();

        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, name => new ApplicationSupportLogger(name));
        }

        public void Dispose()
        {
            _loggers.Values.ToList().ForEach(logger => logger.Dispose());
            _loggers.Clear();
        }
    }

    public static class ApplicationSupportLoggerExtensions
    {
        public static ILoggingBuilder AddApplicationSupportLogger(this ILoggingBuilder builder)
        {
            builder.AddProvider(new ApplicationSupportLoggerProvider());
            return builder;
        }
    }
}
