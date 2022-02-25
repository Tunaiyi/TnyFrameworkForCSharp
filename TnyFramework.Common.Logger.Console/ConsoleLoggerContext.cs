using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;


namespace TnyFramework.Common.Logger.Console
{
    public class ConsoleLoggerContext : LoggerContext
    {
        private ConsoleLoggerContext()
        {
        }


        public static void Init()
        {
            LogFactory.DefaultFactory = CreateConsoleFactory();
        }


        private static ILoggerFactory CreateConsoleFactory()
        {
            return LoggerFactory.Create(builder => {
                builder.AddSimpleConsole(op => {
                    op.IncludeScopes = true;
                    op.ColorBehavior = LoggerColorBehavior.Disabled;
                    // op.ColorBehavior = LoggerColorBehavior.Disabled;
                    op.TimestampFormat = "[hh.mm.ss] ";
                });
            });
        }
    }
}
