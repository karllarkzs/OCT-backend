using System;
using Microsoft.Extensions.Logging;

namespace PharmaBack.TrayApp.Logging;

public class GuiLoggerProvider : ILoggerProvider
{
    private readonly Action<string> _logAction;

    public GuiLoggerProvider(Action<string> logAction)
    {
        _logAction = logAction;
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new GuiLogger(_logAction, categoryName);
    }

    public void Dispose() { }

    private class GuiLogger : ILogger
    {
        private readonly Action<string> _logAction;
        private readonly string _category;

        public GuiLogger(Action<string> logAction, string category)
        {
            _logAction = logAction;
            _category = category;
        }

        public IDisposable BeginScope<TState>(TState state) => default!;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter
        )
        {
            var message = formatter(state, exception);
            var logEntry = $"[{logLevel}] {_category}: {message}";

            if (exception != null)
                logEntry += $"\nException: {exception}";

            _logAction(logEntry);
        }
    }
}
