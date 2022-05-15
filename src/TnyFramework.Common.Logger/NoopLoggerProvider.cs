using Microsoft.Extensions.Logging;

namespace TnyFramework.Common.Logger
{

    public class NoopLoggerProvider : ILoggerProvider
    {
        public void Dispose()
        {
        }

        public ILogger CreateLogger(string categoryName)
        {
            return NoopLogger.INSTANCE;
        }
    }

}
