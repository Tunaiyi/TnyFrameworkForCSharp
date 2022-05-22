using TnyFramework.Net.Command;
using TnyFramework.Net.Endpoint;
using TnyFramework.Net.Message;
using TnyFramework.Net.Rpc;

namespace TnyFramework.Net.Base
{

    public interface INetworkContext : IEndpointContext, IRpcContext
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
        /// 消息工厂
        /// </summary>
        IMessageFactory MessageFactory { get; }

        /// <summary>
        /// 消息者工厂
        /// </summary>
        IMessagerFactory MessagerFactory { get; }

        /// <summary>
        /// 服务配置¬
        /// </summary>
        IServerBootstrapSetting Setting { get; }
    }

}
