using Microsoft.Extensions.Configuration;
using TnyFramework.Net.Base;
using TnyFramework.Net.DotNetty.Bootstrap;
namespace TnyFramework.Net.DotNetty.AspNetCore
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
