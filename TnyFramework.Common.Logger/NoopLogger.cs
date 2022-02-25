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
            var message = (object)formatter(state, exception) ?? state;
            message = FormatMessage(logLevel, message);
            if (message != null)
                Console.WriteLine(message);
            if (exception != null)
                Console.WriteLine(exception);
        }


        public bool IsEnabled(LogLevel logLevel)
        {
            Console.WriteLine($"IsEnabled {logLevel}");
            return logLevel != LogLevel.None;
        }
        
        private object FormatMessage(LogLevel level, object msg)
        {
            switch (level)
            {
                case LogLevel.Trace:
                    return "<color=#bfbfbf>" + msg + "</color>";
                case LogLevel.Debug:
                    return "<color=#8c8c8c>" + msg + "</color>";
                case LogLevel.Information:
                    return "<color=green>" + msg + "</color>";
                case LogLevel.Warning:
                    return "<color=yellow>" + msg + "</color>";
                case LogLevel.Error:
                    return "<color=#FF5555>" + msg + "</color>";
                case LogLevel.Critical:
                    return "<color=red>" + msg + "</color>";
                case LogLevel.None:
                default:
                    return msg;
            }
        }


        public IDisposable BeginScope<TState>(TState state)
        {
            throw new NotImplementedException();
        }
    }
}
