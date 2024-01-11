// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using TnyFramework.Net.Extensions;
using TnyFramework.Net.Nats.Options;

namespace TnyFramework.Net.Nats.Core
{

    public class NetAccessNode : BaseAccessNode
    {
        public static INatsAccessNode Create(IRpcServerOptions server, long serverId, long accessId)
        {
            var one = new NetAccessNode();
            one.Init(server.ServiceName(), serverId, accessId);
            return one;
        }

        public static INatsAccessNode Create(string key)
        {
            var one = new NetAccessNode();
            one.Init(key);
            return one;
        }

        public static INatsAccessNode Create(string accessType, long nodeId, long accessId)
        {
            var one = new NetAccessNode();
            one.Init(accessType, nodeId, accessId);
            return one;
        }

        public static INatsAccessNode Create(INatsAccessNode access)
        {
            var one = new NetAccessNode();
            one.Init(access);
            return one;
        }

        private bool Equals(INatsAccessNode other)
        {
            return NodeKey == other.NodeKey;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((INatsAccessNode) obj);
        }

        public override int GetHashCode()
        {
            return NodeKey.GetHashCode();
        }

        public override string ToString()
        {
            return $"{nameof(NetAccessNode)}[{NodeKey}]";
        }

    }

}
