using TnyFramework.Common.Attribute;
namespace TnyFramework.Net.Base
{
    public class NetAppContext : AttributesContext, INetAppContext
    {
        public string Name { get; set; }

        public int ServerId { get; set; }

        public string AppType { get; set; } = "default";

        public string ScopeType { get; set; } = "online";

        public string Locale { get; set; } = "zh-CN";
    }
}
