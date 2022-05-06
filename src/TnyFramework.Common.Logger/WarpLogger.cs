#region

using System;
using Microsoft.Extensions.Logging;

#endregion

namespace TnyFramework.Common.Logger
{

    internal class WarpLogger : ILogger
    {
        private volatile ILogger logger;

        private readonly string name;


        public WarpLogger(string name)
        {
            this.name = name;
        }


        private ILogger Logger => logger ?? (logger = LogFactory.DefaultFactory.CreateLogger(name));


        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            Logger.Log(logLevel, eventId, state, exception, formatter);
        }


        public bool IsEnabled(LogLevel logLevel)
        {
            return Logger.IsEnabled(logLevel);
        }


        public IDisposable BeginScope<TState>(TState state)
        {
            return Logger.BeginScope(state);
        }
    }

}
