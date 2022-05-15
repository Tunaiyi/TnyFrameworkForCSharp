using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using UnityEngine;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace TnyFramework.Common.Logger.Unity
{

    public class UnityLoggerProvider : ILoggerProvider
    {
        private static readonly ConcurrentDictionary<string, UnityLogger> LOGGERS = new ConcurrentDictionary<string, UnityLogger>();

        public ILogger CreateLogger(string categoryName)
        {
            Debug.unityLogger.Log(LogType.Log, $"Get Logger {categoryName}");
            return LOGGERS.GetOrAdd(categoryName, name => new UnityLogger(name));
        }

        public void Dispose()
        {
            // LOGGERS.Clear();
        }
    }

}
