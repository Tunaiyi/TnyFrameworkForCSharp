using System.Net;
using System.Threading;
using DotNetty.Transport.Channels;
using TnyFramework.Common.Attribute;
using TnyFramework.Net.Transport;

namespace TnyFramework.Net.DotNetty.Transport
{

    public class NettyChannelConnection : AttributesContext, IConnection
    {
        protected readonly IChannel channel;

        private const int STATUS_CLOSE = 0;
        private const int STATUS_OPEN = 1;

        private volatile int status = STATUS_OPEN;

        protected NettyChannelConnection(IChannel channel)
        {
            this.channel = channel;
        }

        public EndPoint RemoteAddress => channel.RemoteAddress;

        public EndPoint LocalAddress => channel.LocalAddress;

        public bool IsActive()
        {
            var current = channel;
            return current != null && current.Active;
        }

        public bool IsClosed()
        {
            return status == STATUS_CLOSE;
        }

        public bool Close()
        {
            if (status == STATUS_CLOSE)
            {
                return false;
            }
            if (Interlocked.CompareExchange(ref status, STATUS_OPEN, STATUS_CLOSE) != STATUS_OPEN)
                return false;
            DoClose();
            return true;
        }

        protected virtual void DoClose()
        {
        }

        public override string ToString()
        {
            return $"{channel}";
        }
    }

}
