// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.ObjectPool;

namespace TnyFramework.Net.Nats.Core
{

    public class NatsAccessor : BaseAccessNode, INatsAccessor, IAsyncDisposable, IDisposable
    {
        private static readonly ObjectPool<NatsAccessor> OBJECT_POOL =
            new DefaultObjectPool<NatsAccessor>(new DefaultPooledObjectPolicy<NatsAccessor>());

        public static string OfKey(string accessType, long nodeId, long accessId, long channelId)
        {
            return $"{accessType}#{nodeId}#{accessId}#{channelId}";
        }

        public static string OfKey(INatsAccessNode node, long channelId)
        {
            return $"{node.AccessType}#{node.NodeId}#{node.AccessId}#{channelId}";
        }

        public static INatsAccessor Create(string key)
        {
            var one = OBJECT_POOL.Get();
            one.Init(key);
            return one;
        }

        public static INatsAccessor Create(INatsAccessNode node, long channelId)
        {
            var one = OBJECT_POOL.Get();
            one.Init(node, channelId);
            return one;
        }

        public static INatsAccessor Create(string accessType, long nodeId, long accessId, long channelId)
        {
            var one = OBJECT_POOL.Get();
            one.Init(accessType, nodeId, accessId, channelId);
            return one;
        }

        public static INatsAccessor Create(INatsAccessor access)
        {
            var one = OBJECT_POOL.Get();
            one.Init(access);
            return one;
        }

        public static INatsAccessor Of(INatsAccessor access)
        {
            if (access is NatsAccessor ins)
            {
                return ins;
            }
            var one = OBJECT_POOL.Get();
            one.Init(access);
            return one;
        }

        public string AccessKey { get; private set; } = "";

        public long ChannelId { get; private set; }

        protected override void Init(string key)
        {
            var worlds = key.Split("#");
            var lastIndex = key.LastIndexOf("#", StringComparison.Ordinal);
            DoInit(key.Substring(0, lastIndex), worlds);
            ChannelId = long.Parse(worlds[3]);
            AccessKey = key;
        }

        private void Init(string accessType, long nodeId, long accessId, long channelId)
        {
            base.Init(accessType, nodeId, accessId);
            ChannelId = channelId;
            AccessKey = OfKey(accessType, nodeId, accessId, channelId);
        }

        private void Init(INatsAccessNode node, long channelId)
        {
            Init(node.AccessType, node.NodeId, node.AccessId, channelId);
        }

        private void Init(INatsAccessor access)
        {
            Init(access.AccessType, access.NodeId, access.AccessId, access.ChannelId);
        }

        private bool Equals(INatsAccessor other)
        {
            return AccessKey == other.AccessKey;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((NatsAccessor) obj);
        }

        public override int GetHashCode()
        {
            return AccessKey.GetHashCode();
        }

        protected override void Clear()
        {
            base.Clear();
            ChannelId = 0;
            AccessKey = "";
        }

        public void Dispose()
        {
            Clear();
            OBJECT_POOL.Return(this);
        }

        public ValueTask DisposeAsync()
        {
            OBJECT_POOL.Return(this);
            return ValueTask.CompletedTask;
        }

        public override string ToString()
        {
            return $"{nameof(NatsAccessor)}[{AccessKey}]";
        }
    }

}
