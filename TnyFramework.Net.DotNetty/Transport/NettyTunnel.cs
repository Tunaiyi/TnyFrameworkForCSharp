using DotNetty.Transport.Channels;
using TnyFramework.Net.DotNetty.Common;
using TnyFramework.Net.DotNetty.Message;
namespace TnyFramework.Net.DotNetty.Transport
{
    public class NettyTunnel : INetTunnel
    {
        private int sendIdCounter = 0;
        
        public IOnMessage OnMessage { get; set; }

        private IChannel Channel { get; set; }


        public NettyTunnel(IChannel channel, IMessageFactory messageFactory)
        {
            Channel = channel;
            MessageFactory = messageFactory;
        }


        public long AccessId { get; private set; }
        public TunnelMode Mode { get; }


        public void SetAccessId(long accessId)
        {
            if (AccessId == 0)
            {
                AccessId = accessId;
            }
        }


        public IMessageFactory MessageFactory { get; }


        public bool Open()
        {
            Channel.GetAttribute(NettyNetAttrKeys.TUNNEL).Set(this);
            return true;
        }


        public bool Close()
        {
            return true;
        }


        public bool Disconnect()
        {
            return true;
        }


        public bool Receive(IMessage message)
        {
            OnMessage.OnMessage(this, message);
            return true;
        }


        public ISendReceipt Send(MessageContext messageContext)
        {
            var message = MessageFactory.Create(++sendIdCounter, messageContext);
            var task = Channel.WriteAndFlushAsync(message);
            if (messageContext is DefaultMessageContext context)
            {
                context.WrittenTask = task;
            }
            return messageContext;
        }
    }
}
