using TnyFramework.Common.Logger;
using Xunit.Abstractions;
using XunitFactory = Divergic.Logging.Xunit.LogFactory;

namespace TnyFramework.Common.Test
{

    public static class XUnitLoggerFactory
    {
        public static void InitFactory(ITestOutputHelper output)
        {
            LogFactory.DefaultFactory = XunitFactory.Create(output);
        }
    }

}
