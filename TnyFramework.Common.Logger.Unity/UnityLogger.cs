using System;
using Microsoft.Extensions.Logging;
using UnityEngine;
using ILogger = Microsoft.Extensions.Logging.ILogger;
namespace TnyFramework.Common.Logger.Unity
{
    public class UnityLogger : ILogger
    {
        private string categoryName;


        public UnityLogger(string categoryName)
        {
            this.categoryName = categoryName;
        }


        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
                return;
            if (formatter == null)
                throw new ArgumentNullException(nameof(formatter));
            var message = (object)formatter(state, exception) ?? state;
            message = FormatMessage(logLevel, message);
            if (message != null)
                Debug.unityLogger.Log(ToLogType(logLevel), message);
            if (exception != null)
                Debug.unityLogger.LogException(exception);
        }


        private object FormatMessage(LogLevel level, object msg)
        {
            switch (level)
            {
                case LogLevel.Trace:
                    return "<color='#BFBFBF'>" + msg + "</color>";
                case LogLevel.Debug:
                    return "<color='#8c8c8c'>" + msg + "</color>";
                case LogLevel.Information:
                    return "<color=green>" + msg + "</color>";
                case LogLevel.Warning:
                    return "<color=yellow>" + msg + "</color>";
                case LogLevel.Error:
                    return "<color='#FF3333'>" + msg + "</color>";
                case LogLevel.Critical:
                    return "<color=red>" + msg + "</color>";
                case LogLevel.None:
                default:
                    return msg;
            }
        }


        public bool IsEnabled(LogLevel logLevel)
        {
            Debug.unityLogger.Log(LogType.Log, $"IsEnabled {logLevel}");
            return logLevel != LogLevel.None;
            // if (logLevel == LogLevel.None)
            //     return false;
            // if (logLevel == LoggerContext.Level)
            //     return true;
            // switch (LoggerContext.Level)
            // {
            //     case LogLevel.Trace:
            //         return logLevel == LogLevel.Trace;
            //     case LogLevel.Debug:
            //         return logLevel == LogLevel.Trace ||
            //                logLevel == LogLevel.Debug;
            //     case LogLevel.Information:
            //         return logLevel == LogLevel.Trace ||
            //                logLevel == LogLevel.Debug ||
            //                logLevel == LogLevel.Information;
            //     case LogLevel.Warning:
            //         return logLevel == LogLevel.Trace ||
            //                logLevel == LogLevel.Debug ||
            //                logLevel == LogLevel.Information||
            //                logLevel == LogLevel.Warning;
            //     case LogLevel.Error:
            //         return logLevel == LogLevel.Trace ||
            //                logLevel == LogLevel.Debug ||
            //                logLevel == LogLevel.Information ||
            //                logLevel == LogLevel.Warning||
            //                logLevel == LogLevel.Error;
            //     case LogLevel.Critical:
            //         return logLevel == LogLevel.Trace ||
            //                logLevel == LogLevel.Debug ||
            //                logLevel == LogLevel.Information ||
            //                logLevel == LogLevel.Warning ||
            //                logLevel == LogLevel.Error||
            //                logLevel == LogLevel.Critical;
            //     case LogLevel.None:
            //     default:
            //         return false;
            // }
        }


        public IDisposable BeginScope<TState>(TState state)
        {
            throw new NotImplementedException();
        }


        private static LogType ToLogType(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Trace:
                    return LogType.Log;
                case LogLevel.Debug:
                    return LogType.Log;
                case LogLevel.Information:
                    return LogType.Log;
                case LogLevel.Warning:
                    return LogType.Warning;
                case LogLevel.Error:
                    return LogType.Error;
                case LogLevel.Critical:
                    return LogType.Error;
                case LogLevel.None:
                    return LogType.Exception;
                default:
                    return LogType.Assert;
            }

        }
    }
}
