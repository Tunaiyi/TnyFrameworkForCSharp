using TnyFramework.Net.Command;
using TnyFramework.Net.Endpoint;
using TnyFramework.Net.Message;
namespace TnyFramework.Net.Base
{
    public interface INetworkContext : IEndpointContext
    {
        /// <summary>
        /// 认证工厂
        /// </summary>
        ICertificateFactory GetCertificateFactory();


        /// <summary>
        /// 认证工厂
        /// </summary>
        ICertificateFactory<TUserId> CertificateFactory<TUserId>();


        /// <summary>
        /// 消息公共方法
        /// </summary>
        IMessageFactory MessageFactory { get; }

        /// <summary>
        /// 服务配置¬
        /// </summary>
        IServerBootstrapSetting Setting { get; }
    }
}
