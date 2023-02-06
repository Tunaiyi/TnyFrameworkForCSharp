// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

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
            : base(id, transporter, NetAccessMode.Server, context)
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
