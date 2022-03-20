using System.Net;
using System.Threading.Tasks;
using DotNetty.Transport.Channels;
using TnyFramework.Common.Attribute;
using TnyFramework.Common.Event;
using TnyFramework.Net.Base;
using TnyFramework.Net.Command;
using TnyFramework.Net.DotNetty.Common;
using TnyFramework.Net.Endpoint;
using TnyFramework.Net.Message;
using TnyFramework.Net.Rpc;
using TnyFramework.Net.Transport;
using TnyFramework.Net.Transport.Event;
namespace TnyFramework.Net.DotNetty.Transport
{
    public class NettyTunnel : INetTunnel
    {
        private int sendIdCounter = 0;

        public IOnMessage OnMessage { get; set; }

        private IChannel Channel { get; set; }


        public NettyTunnel(IChannel channel, IMessageFactory messageFactory, INetworkContext context)
        {
            Channel = channel;
            MessageFactory = messageFactory;
            Context = context;

        }


        public long Id { get; }
        public long AccessId { get; private set; }
        public TunnelMode Mode { get; }
        public TunnelStatus Status { get; }


        public bool IsActive()
        {
            throw new System.NotImplementedException();
        }


        public bool IsOpen()
        {
            throw new System.NotImplementedException();
        }


        public EndPoint RemoteAddress { get; }
        public EndPoint LocalAddress { get; }


        IEndpoint ITunnel.GetEndpoint()
        {
            return GetEndpoint();
        }


        public IAttributes Attributes { get; }
        public object UserId { get; }


        public void SetAccessId(long accessId)
        {
            if (AccessId == 0)
            {
                AccessId = accessId;
            }
        }


        public IMessageFactory MessageFactory { get; }
        public IEndpoint Endpoint { get; }


        public INetEndpoint GetEndpoint()
        {
            throw new System.NotImplementedException();
        }


        public ICertificateFactory CertificateFactory { get; }


        public object GetUserId()
        {
            throw new System.NotImplementedException();
        }


        public string UserType { get; }
        public ICommunicator Certificate { get; }


        public bool Bind(INetEndpoint endpoint)
        {
            throw new System.NotImplementedException();
        }


        public bool Open()
        {
            Channel.GetAttribute(NettyNetAttrKeys.TUNNEL).Set(this);
            return true;
        }


        public bool IsClosed()
        {
            throw new System.NotImplementedException();
        }


        public bool Close()
        {
            return true;
        }


        public void Disconnect()
        {
        }


        public void Reset()
        {
            throw new System.NotImplementedException();
        }


        public ICertificate GetCertificate()
        {
            throw new System.NotImplementedException();
        }


        public bool IsAuthenticated()
        {
            throw new System.NotImplementedException();
        }


        public void Pong()
        {
            throw new System.NotImplementedException();
        }


        public void Ping()
        {
            throw new System.NotImplementedException();
        }


        public INetworkContext Context { get; }


        public IEventBox<TunnelActivate> ActivateEvent { get; }
        public IEventBox<TunnelUnactivated> UnactivatedEvent { get; }
        public IEventBox<TunnelClose> CloseEvent { get; }


        public bool Receive(IMessage message)
        {
            OnMessage.OnMessage(this, message);
            return true;
        }


        public ISendReceipt Send(MessageContext messageContext)
        {
            var message = MessageFactory.Create(++sendIdCounter, messageContext);
            var task = Channel.WriteAndFlushAsync(message);
            if (messageContext is IMessageWritableContext context)
            {
                context.SetWrittenTask(task);
            }
            return messageContext;
        }


        public Task Write(IMessage message)
        {
            throw new System.NotImplementedException();
        }


        public Task Write(MessageAllocator allocator, MessageContext messageContext)
        {
            throw new System.NotImplementedException();
        }


        public Task Write(IMessageAllocator allocator, MessageContext messageContext)
        {
            throw new System.NotImplementedException();
        }
    }
}
