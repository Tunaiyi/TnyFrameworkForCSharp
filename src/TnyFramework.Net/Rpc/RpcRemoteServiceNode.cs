using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using TnyFramework.Common.Extensions;
using TnyFramework.Net.Endpoint;

namespace TnyFramework.Net.Rpc
{

    public class RpcRemoteServiceNode : IRpcRemoteNode, IComparable<RpcRemoteServiceNode>
    {
        private readonly RpcRemoteServiceSet service;

        private readonly IDictionary<long, RpcRemoteServiceAccess> remoteServiceAccessMap = new Dictionary<long, RpcRemoteServiceAccess>();

        private volatile IList<IRpcRemoteAccess> orderAccessPoints = ImmutableList.Create<IRpcRemoteAccess>();

        private readonly ReaderWriterLockSlim rwLock = new ReaderWriterLockSlim();

        public int ServerId { get; }

        public bool IsActive() => !orderAccessPoints.IsEmpty();

        public int NodeId => ServerId;

        public IRpcServiceType ServiceType { get; }

        private void ReadLock() => rwLock.EnterReadLock();

        private void ReadUnlock() => rwLock.ExitReadLock();

        private void WriteLock() => rwLock.EnterWriteLock();

        private void WriteUnlock() => rwLock.ExitWriteLock();

        public RpcRemoteServiceNode(int serverId, RpcRemoteServiceSet service)
        {
            this.service = service;
            ServiceType = service.ServiceType;
            ServerId = serverId;
        }

        public IList<IRpcRemoteAccess> GetOrderRemoteAccesses()
        {
            return orderAccessPoints;
        }

        public IRpcRemoteAccess GetRemoteAccess(long accessId)
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
            orderAccessPoints = remoteServiceAccessMap.Values.OfType<IRpcRemoteAccess>().ToImmutableSortedSet();
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

        public int CompareTo(RpcRemoteServiceNode other)
        {
            if (ReferenceEquals(this, other)) return 0;
            return ReferenceEquals(null, other) ? 1 : ServerId.CompareTo(other.ServerId);
        }
    }

}
