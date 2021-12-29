using System;
using System.Threading;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using static System.Threading.Volatile;


namespace TnyFramework.Common.Logger
{
    public static class LogFactory
    {
        private static ILoggerFactory _DEFAULT_FACTORY;

        public static ILoggerFactory DefaultFactory {
            get {
                var current = Read(ref _DEFAULT_FACTORY);
                if (current != null)
                    return current;
                current = CreateConsoleFactory();
                var old = Interlocked.CompareExchange(ref _DEFAULT_FACTORY, current, (ILoggerFactory)null);
                return old ?? current;
            }
            set => Write(ref _DEFAULT_FACTORY, value);
        }


        private static ILoggerFactory CreateConsoleFactory()
        {
            return LoggerFactory.Create(builder => {
                builder.AddSimpleConsole(op => {
                    op.IncludeScopes = true;
                    op.ColorBehavior = LoggerColorBehavior.Disabled;
                    op.TimestampFormat = "[hh.mm.ss] ";
                });
            });
        }


        /// <summary>
        ///     Creates a new logger instance with the name of the specified type.
        /// </summary>
        /// <typeparam name="T">type where logger is used</typeparam>
        /// <returns>logger instance</returns>
        public static ILogger Logger<T>() => Logger(typeof(T));


        /// <summary>
        ///     Creates a new logger instance with the name of the specified type.
        /// </summary>
        /// <param name="type">type where logger is used</param>
        /// <returns>logger instance</returns>
        public static ILogger Logger(Type type) => Logger(type.FullName);


        /// <summary>
        ///     Creates a new logger instance with the specified name.
        /// </summary>
        /// <param name="name">logger name</param>
        /// <returns>logger instance</returns>
        public static ILogger Logger(string name) => DefaultFactory.CreateLogger(name);
    }
}
