using Microsoft.Extensions.Configuration;
using TnyFramework.Net.Base;

namespace TnyFramework.Net.DotNetty.NetCore
{

    public class NetApplicationHostOptions
    {
        public static readonly string ROOT_PATH = ConfigurationPath.Combine("Tny", "Net");

        public string Name { get; set; }

        public int ServerId { get; set; }

        public string AppType { get; set; } = "default";

        public string ScopeType { get; set; } = "online";

        public string Locale { get; set; } = "zh-CN";

        public ServerSetting Server { get; set; }

        public ServerSetting RpcServer { get; set; }
    }

}
