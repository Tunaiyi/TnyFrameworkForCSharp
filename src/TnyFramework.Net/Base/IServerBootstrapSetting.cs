using TnyFramework.Common.Extension;
namespace TnyFramework.Net.Base
{
    public interface IServerBootstrapSetting : INetBootstrapSetting
    {
        /// <summary>
        /// 绑定域名
        /// </summary>
        string Host { get; }

        /// <summary>
        /// 绑定端口
        /// </summary>
        int Port { get; }

        /// <summary>
        /// 协议
        /// </summary>
        string Scheme { get; }
        
    }

    public static class ServerBootstrapSettingExtensions
    {
        public static string ServiceName(this IServerBootstrapSetting self)
        {
            return self.Name.IsBlank() ? self.ServeName : self.Name;
        }
    }
}
