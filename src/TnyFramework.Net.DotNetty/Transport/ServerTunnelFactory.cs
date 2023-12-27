// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using DotNetty.Transport.Channels;
using TnyFramework.Net.Application;
using TnyFramework.Net.Transport;

namespace TnyFramework.Net.DotNetty.Transport
{

    public class ServerTunnelFactory : INettyTunnelFactory
    {
        public INetTunnel Create(long id, IChannel channel, INetworkContext context, INetService service)
        {
            var transport = new NettyChannelMessageTransporter(NetAccessMode.Server, channel);
            return new ServerTunnel<NettyChannelMessageTransporter>(id, transport, context, service); // 创建 Tunnel 已经transport.bind
        }
    }

}
