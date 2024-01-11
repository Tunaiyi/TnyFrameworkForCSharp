// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using TnyFramework.Net.Application;
using TnyFramework.Net.Extensions;
using TnyFramework.Net.Nats.Core;

namespace TnyFramework.Net.Nats.Options.Extensions
{

    public static class PubSubRpcOptionsExtensions
    {
        public static RpcTopicBuilder TopicBuilder(this IPubSubRpcOptions options)
        {
            return new RpcTopicBuilder(options.TopicSeparator, options.TopicPrefix, 128);
        }

        public static string GetChannelTopic(this IPubSubRpcOptions options, string accessType, long serverId, long accessId, bool wildcard = false)
        {
            return options.GetTopic(options.TopicChannelToken, accessType, serverId, accessId, wildcard);
        }

        public static string GetChannelTopic(this IPubSubRpcOptions options, INatsAccessNode node, bool wildcard = false)
        {
            return options.GetTopic(options.TopicChannelToken, node, wildcard);
        }

        private static string GetTopic(this IPubSubRpcOptions options, string usage, IServiceSetting server,
            long serverId, long accessId, bool wildcard = false)
        {
            return options.GetTopic(usage, server.ServiceName(), serverId, accessId, wildcard);
        }

        private static string GetTopic(this IPubSubRpcOptions options, string usage, INatsAccessNode node,
            bool wildcard = false)
        {
            return options.GetTopic(usage, node.AccessType, node.NodeId, node.AccessId, wildcard);
        }

        private static string GetTopic(this IPubSubRpcOptions options, string usage, string accessType, long nodeId,
            long? accessId, bool wildcard = false)
        {
            var builder = TopicBuilder(options)
                .Concat(accessType)
                .Concat(nodeId)
                .Concat(usage);
            if (accessId != null)
            {
                builder.Concat(accessId);
            }
            if (wildcard)
            {
                builder.Concat("*");
            }
            return builder.ToString();
        }
    }

}
