// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using TnyFramework.Common.Extensions;
using TnyFramework.Net.Base;
using TnyFramework.Net.Endpoint;

namespace TnyFramework.Net.Rpc
{

    public class RpcServiceNode : IRpcInvokeNode, IComparable<RpcServiceNode>
    {
        private readonly RpcServiceSet service;

        private readonly IDictionary<long, RpcRemoteServiceAccess> remoteServiceAccessMap = new Dictionary<long, RpcRemoteServiceAccess>();

        private volatile IList<IRpcAccess> orderAccessPoints = ImmutableList.Create<IRpcAccess>();

        private readonly ReaderWriterLockSlim rwLock = new ReaderWriterLockSlim();

        public int ServerId { get; }

        public bool IsActive() => !orderAccessPoints.IsEmpty();

        public int NodeId => ServerId;

        public IMessagerType ServiceType => service.ServiceType;

        private void ReadLock() => rwLock.EnterReadLock();

        private void ReadUnlock() => rwLock.ExitReadLock();

        private void WriteLock() => rwLock.EnterWriteLock();

        private void WriteUnlock() => rwLock.ExitWriteLock();

        public RpcServiceNode(int serverId, RpcServiceSet service)
        {
            this.service = service;
            ServerId = serverId;
        }

        public IList<IRpcAccess> GetOrderAccesses()
        {
            return orderAccessPoints;
        }

        public IRpcAccess GetAccess(long accessId)
        {
            ReadLock();
            try
            {
                return remoteServiceAccessMap[accessId];
            } finally
            {
                ReadUnlock();
            }
        }

        internal void AddEndpoint(IEndpoint<RpcAccessIdentify> endpoint)
        {
            WriteLock();
            try
            {
                var activate = remoteServiceAccessMap.IsEmpty();
                var nodeId = endpoint.UserId;
                remoteServiceAccessMap[nodeId.Id] = new RpcRemoteServiceAccess(endpoint);
                Order(activate);
            } finally
            {
                WriteUnlock();
            }
        }

        internal void RemoveEndpoint(IEndpoint<RpcAccessIdentify> endpoint)
        {
            WriteLock();
            try
            {
                var nodeId = endpoint.UserId;
                var activate = this.remoteServiceAccessMap.IsEmpty();
                if (!remoteServiceAccessMap.TryGetValue(nodeId.Id, out var accessPoint))
                    return;
                var exist = accessPoint.Endpoint;
                if (!ReferenceEquals(endpoint, exist))
                    return;
                if (remoteServiceAccessMap.Remove(nodeId.Id))
                {
                    Order(activate);
                }
            } finally
            {
                WriteUnlock();
            }
        }

        private void Order(bool activate)
        {
            orderAccessPoints = remoteServiceAccessMap.Values.OfType<IRpcAccess>().ToImmutableSortedSet();
            switch (activate)
            {
                case true when remoteServiceAccessMap.IsEmpty():
                    service.OnNodeUnactivated(this);
                    break;
                case false when !remoteServiceAccessMap.IsEmpty():
                    service.OnNodeActivate(this);
                    break;
            }
        }

        public int CompareTo(RpcServiceNode? other)
        {
            if (ReferenceEquals(this, other)) return 0;
            return ReferenceEquals(null, other) ? 1 : ServerId.CompareTo(other.ServerId);
        }
    }

}
