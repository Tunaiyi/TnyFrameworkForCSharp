using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TnyFramework.Common.Logger;
using TnyFramework.Net.Base;
using TnyFramework.Net.Endpoint;
using TnyFramework.Net.Exceptions;
using TnyFramework.Net.Message;
namespace TnyFramework.Net.Transport
{
    public abstract class BaseNetTunnel<TUserId, TEndpoint, TTransporter> : NetTunnel<TUserId, TEndpoint>
        where TEndpoint : INetEndpoint<TUserId>
        where TTransporter : IMessageTransporter
    {
        private static readonly ILogger LOGGER = LogFactory.Logger(typeof(BaseNetTunnel<,,>));

        protected TTransporter Transporter { get; }

        public override EndPoint RemoteAddress => Transporter.RemoteAddress;

        public override EndPoint LocalAddress => Transporter.LocalAddress;


        protected BaseNetTunnel(long id, TTransporter transporter, TunnelMode mode, INetworkContext context) : base(id, mode, context)
        {
            if (transporter == null)
                return;
            Transporter = transporter;
            Transporter.Bind(this);
        }


        public override bool IsActive()
        {
            var transporter = Transporter;
            return Status == TunnelStatus.Open && transporter != null && transporter.IsActive();
        }


        public override void Reset()
        {

            if (Status == TunnelStatus.Init)
            {
                return;
            }
            lock (this)
            {
                if (Status == TunnelStatus.Init)
                {
                    return;
                }
                if (!IsActive())
                {
                    DoDisconnect();
                }
                Status = TunnelStatus.Init;
            }
        }


        public override Task Write(IMessage message)
        {
            return CheckAvailable(null, out var task) ? Transporter.Write(message) : task;
        }


        public override Task Write(MessageAllocator allocator, MessageContext messageContext)
        {
            return CheckAvailable(messageContext, out var task) ? Transporter.Write(allocator, MessageFactory, messageContext) : task;
        }


        protected virtual void OnWriteUnavailable()
        {

        }


        protected override void DoDisconnect()
        {
            var transporter = Transporter;
            if (transporter == null || !transporter.IsActive())
                return;
            try
            {
                transporter.Close();
            } catch (Exception e)
            {
                LOGGER.LogError(e, "transporter close error");
            }
        }


        private bool CheckAvailable(MessageContext context, out Task task)
        {
            if (IsActive())
            {
                task = null;
                return true;
            }
            OnWriteUnavailable();
            if (context != null)
            {
                context.Cancel(false);
                task = context.Written();
            } else
            {
                var cause = new TunnelDisconnectException($"{this} is disconnect");
                task = Task.FromException(cause);
            }
            return false;
        }


        public override string ToString()
        {
            return $"{Mode}[{UserGroup}({UserId}) {Transporter}]";
        }
    }
}
