using DotNetty.Transport.Channels;
using TnyFramework.Net.Base;
using TnyFramework.Net.Transport;

namespace TnyFramework.Net.DotNetty.Transport
{

    public class ServerTunnelFactory<TUserId> : INettyTunnelFactory
    {
        public INetTunnel Create(long id, IChannel channel, INetworkContext context)
        {
            var transport = new NettyChannelMessageTransporter(channel);
            return new ServerTunnel<TUserId, NettyChannelMessageTransporter>(id, transport, context); // 创建 Tunnel 已经transport.bind
        }
    }

}
