// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System.Net;
using System.Net.Sockets;
using TnyFramework.Net.Nats.Core;

namespace TnyFramework.Net.Nats.Transports
{

    public class NatsEndPoint : EndPoint
    {
        private readonly string uri;

        public NatsEndPoint(INatsAccessNode node, long channelId)
        {
            uri = $"nats://{node.AccessType}/{node.NodeId}?/{node.AccessId}/{channelId}";
        }

        public NatsEndPoint(INatsAccessor accessor)
        {
            uri = $"nats://{accessor.AccessType}/{accessor.NodeId}/{accessor.AccessId}/{accessor.ChannelId}";
        }

        public override AddressFamily AddressFamily => AddressFamily.Unknown;

        public override string ToString()
        {
            return uri;
        }
    }

}
