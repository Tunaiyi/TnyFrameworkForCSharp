using Microsoft.Extensions.Logging;
namespace TnyFramework.Common.Logger.Unity
{
    public class UnityLoggerContext : LoggerContext
    {
        private static readonly UnityLoggerProvider PROVIDER = new UnityLoggerProvider();


        private UnityLoggerContext()
        {
        }


        public static void Init()
        {
            LogFactory.DefaultFactory = CreateUnityFactory();
        }


        private static ILoggerFactory CreateUnityFactory()
        {
            return LoggerFactory.Create(builder => { builder.AddProvider(PROVIDER); });
        }
    }
}
