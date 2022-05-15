using Microsoft.Extensions.Logging;
using TnyFramework.Common.Logger;
using TnyFramework.Net.Base;
using TnyFramework.Net.Endpoint;

namespace TnyFramework.Net.Transport
{

    public class ServerTunnel
    {
        internal static readonly ILogger LOGGER = LogFactory.Logger<ServerTunnel>();
    }

    public class ServerTunnel<TUserId, TTransporter> : BaseNetTunnel<TUserId, INetSession<TUserId>, TTransporter>
        where TTransporter : IMessageTransporter
    {
        private readonly ILogger logger = ServerTunnel.LOGGER;

        public ServerTunnel(long id, TTransporter transporter, INetworkContext context)
            : base(id, transporter, TunnelMode.SERVER, context)
        {
            var factory = context.CertificateFactory<TUserId>();
            Bind(new AnonymityEndpoint<TUserId>(factory, context, this));
        }

        protected sealed override bool ReplaceEndpoint(INetEndpoint newEndpoint)
        {

            var certificate = Certificate;
            if (certificate.IsAuthenticated())
                return false;
            var commandTaskBox = Endpoint.CommandTaskBox;
            SetEndpoint((INetSession<TUserId>) newEndpoint);
            Endpoint.TakeOver(commandTaskBox);
            return true;
        }

        protected override void OnDisconnect()
        {
            Close();
        }

        protected override bool OnOpen()
        {
            var transporter = Transporter;
            if (transporter != null && transporter.IsActive())
                return true;
            logger.LogWarning("open failed. channel {Transporter} is not active", transporter);
            return false;
        }
    }

}
