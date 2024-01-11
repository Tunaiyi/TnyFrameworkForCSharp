// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System.Net;
using TnyFramework.Net.Application;
using TnyFramework.Net.Nats.Codecs;
using TnyFramework.Net.Session;
using TnyFramework.Net.Transport;

namespace TnyFramework.Net.Nats.Transports
{

    public class NatsTunnel : BaseNetTunnel<INetSession, NatsTransport>
    {
        public override EndPoint? RemoteAddress => Transporter.RemoteAddress;

        public override EndPoint? LocalAddress => Transporter.LocalAddress;

        internal NatsTunnel(long id, NatsTransport transport, NetAccessMode accessMode, INetworkContext context, INetService service)
            : base(id, transport, accessMode, context, service)
        {
        }

        public NatsUri Uri() => Transporter.Uri;

        protected override void OnWriteUnavailable()
        {
        }

        protected override void OnDisconnect()
        {
            _ = Transporter.Close();
        }

        protected override void OnDisconnected()
        {
            _ = Transporter.Close();
        }

        protected override void OnClosed()
        {
        }

        protected override void OnClose()
        {
        }
    }

}
