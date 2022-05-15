using System.Threading.Tasks;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.Logging;
using TnyFramework.Common.Logger;
using TnyFramework.Net.DotNetty.Common;
using TnyFramework.Net.Endpoint;
using TnyFramework.Net.Message;
using TnyFramework.Net.Transport;

namespace TnyFramework.Net.DotNetty.Transport
{

    public class NettyChannelMessageTransporter : NettyChannelConnection, IMessageTransporter
    {
        private static readonly ILogger LOGGER = LogFactory.Logger<NettyChannelMessageTransporter>();

        public NettyChannelMessageTransporter(IChannel channel) : base(channel)
        {
        }

        public void Bind(INetTunnel tunnel)
        {
            channel?.GetAttribute(NettyNetAttrKeys.TUNNEL).Set(tunnel);
        }

        protected override void DoClose()
        {
            var tunnelAttr = channel?.GetAttribute(NettyNetAttrKeys.TUNNEL);
            var tunnel = tunnelAttr?.Get();
            tunnelAttr?.Remove();
            if (tunnel != null && (tunnel.IsOpen() || tunnel.IsActive()))
            {
                tunnel.Disconnect();
            }
        }

        public Task Write(IMessage message)
        {
            return channel.WriteAndFlushAsync(message);
        }

        public Task Write(IMessageAllocator maker, IMessageFactory factory, MessageContext context)
        {
            return Write(maker.Allocate, factory, context);
        }

        public Task Write(MessageAllocator maker, IMessageFactory factory, MessageContext context)
        {
            return channel.EventLoop.SubmitAsync<object>(() => {
                IMessage message = null;
                try
                {
                    message = maker(factory, context);
                } catch (System.Exception e)
                {
                    context.Cancel(e);
                    LOGGER.LogError(e, "");
                }
                var task = channel.WriteAndFlushAsync(message);
                if (context is IMessageWritableContext ctx)
                    ctx.SetWrittenTask(task);
                return default;
            });
        }
    }

}
