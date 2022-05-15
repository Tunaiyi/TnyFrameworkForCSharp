using System;
using Microsoft.Extensions.Logging;

namespace TnyFramework.Common.Logger
{

    public class NoopLogger : ILogger
    {
        public static readonly NoopLogger INSTANCE = new NoopLogger();

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
                return;
            if (formatter == null)
                throw new ArgumentNullException(nameof(formatter));
            var message = (object) formatter(state, exception) ?? state;
            message = FormatMessage(logLevel, message);
            if (message != null)
                Console.WriteLine(message);
            if (exception != null)
                Console.WriteLine(exception);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel != LogLevel.None;
        }

        private object FormatMessage(LogLevel level, object msg)
        {
            return msg;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            throw new NotImplementedException();
        }
    }

}
