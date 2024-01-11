// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using TnyFramework.Common.Exceptions;
using TnyFramework.Net.Extensions;
using TnyFramework.Net.Nats.Core;

namespace TnyFramework.Net.Nats.Codecs
{

    public class NatsUri : Uri
    {
        private const string SEPARATOR = "/";

        public NatsUri(INatsAccessNode node)
            : this(node.AccessType, node.NodeId, node.AccessId)
        {
        }

        public NatsUri(INatsAccessor accessor)
            : this(accessor.AccessType, accessor.NodeId, accessor.AccessId, accessor.ChannelId)
        {
        }

        public NatsUri(string accessType, long nodeId, long accessId, long? channelId = null)
            : base($"nats://{accessType}/{nodeId}/{accessId}{(channelId == null ? string.Empty : SEPARATOR + channelId)}")
        {
            AccessType = accessType;
            NodeId = nodeId;
            AccessId = accessId;
            ChannelId = channelId;
        }

        public NatsUri(Uri uri) : this(uri.ToString())
        {
        }

        public NatsUri(string url)
            : base(url)
        {
            var pathSegments = this.GetPathSegments();
            if (pathSegments.Length < 2)
            {
                throw new IllegalArgumentException();
            }
            AccessType = Host;
            NodeId = long.Parse(pathSegments[0]);
            AccessId = long.Parse(pathSegments[1]);
            if (pathSegments.Length >= 3)
            {
                ChannelId = long.Parse(pathSegments[2]);
            }
        }

        public string AccessType { get; }

        public long NodeId { get; }

        public long AccessId { get; }

        public long? ChannelId { get; } = 0;

        public INatsAccessNode ToNode()
        {
            return NetAccessNode.Create(AccessType, NodeId, AccessId);
        }

        public INatsAccessor? ToAccessor()
        {
            return ChannelId != null ? NatsAccessor.Create(AccessType, NodeId, AccessId, ChannelId.Value) : null;
        }
    }

}
