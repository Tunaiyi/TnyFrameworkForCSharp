using DotNetty.Transport.Channels;
using TnyFramework.Net.Base;
using TnyFramework.Net.Transport;

namespace TnyFramework.Net.DotNetty.Transport
{

    /// <summary>
    /// Netty Tunnel 工厂
    /// </summary>
    public interface INettyTunnelFactory
    {
        INetTunnel Create(long id, IChannel channel, INetworkContext context);
    }

}
