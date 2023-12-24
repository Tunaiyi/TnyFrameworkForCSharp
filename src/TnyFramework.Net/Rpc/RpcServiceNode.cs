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
using TnyFramework.Net.Application;
using TnyFramework.Net.Endpoint;

namespace TnyFramework.Net.Rpc
{

    public class RpcServiceNode(long serverId, RpcServiceSet service)
        : IRpcInvokeNode, IComparable<RpcServiceNode>
    {
        private readonly IDictionary<long, RpcRemoteServiceAccess> remoteServiceAccessMap =
            new Dictionary<long, RpcRemoteServiceAccess>();

        private volatile IList<IRpcAccess> orderAccessPoints = ImmutableList.Create<IRpcAccess>();

        private readonly ReaderWriterLockSlim rwLock = new();

        public long ServerId { get; } = (int) serverId;

        public bool IsActive() => !orderAccessPoints.IsEmpty();

        public long NodeId => ServerId;

        public IContactType ServiceType => service.ServiceType;

        private void ReadLock() => rwLock.EnterReadLock();

        private void ReadUnlock() => rwLock.ExitReadLock();

        private void WriteLock() => rwLock.EnterWriteLock();

        private void WriteUnlock() => rwLock.ExitWriteLock();

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

        internal void AddEndpoint(IEndpoint endpoint)
        {
            WriteLock();
            try
            {
                var activate = remoteServiceAccessMap.IsEmpty();
                remoteServiceAccessMap[endpoint.Identify] = new RpcRemoteServiceAccess(endpoint);
                Order(activate);
            } finally
            {
                WriteUnlock();
            }
        }

        internal void RemoveEndpoint(IEndpoint endpoint)
        {
            WriteLock();
            try
            {
                var activate = remoteServiceAccessMap.IsEmpty();
                if (!remoteServiceAccessMap.TryGetValue(endpoint.Identify, out var accessPoint))
                    return;
                var exist = accessPoint.Endpoint;
                if (!ReferenceEquals(endpoint, exist))
                    return;
                if (remoteServiceAccessMap.Remove(endpoint.Identify))
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
