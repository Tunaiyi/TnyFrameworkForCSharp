using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using TnyFramework.Common.Extensions;
using TnyFramework.Net.Endpoint;

namespace TnyFramework.Net.Rpc
{

    public class RpcRemoteNode : IComparable<RpcRemoteNode>
    {
        private readonly RpcRemoteServiceSet service;

        private readonly IDictionary<long, RpcEndpointAccessPoint> realEndpointMap = new Dictionary<long, RpcEndpointAccessPoint>();

        private volatile IList<IRpcRemoteAccessPoint> orderAccessPoints = ImmutableList.Create<IRpcRemoteAccessPoint>();

        public int ServerId { get; }

        public IList<IRpcRemoteAccessPoint> OrderAccessPoints => orderAccessPoints;

        public bool IsActive() => !orderAccessPoints.IsEmpty();

        public RpcRemoteNode(int serverId, RpcRemoteServiceSet service)
        {
            this.service = service;
            ServerId = serverId;
        }

        internal void AddEndpoint(IEndpoint<RpcAccessIdentify> endpoint)
        {
            lock (this)
            {
                var activate = realEndpointMap.IsEmpty();
                var nodeId = endpoint.UserId;
                realEndpointMap[nodeId.Id] = new RpcEndpointAccessPoint(endpoint);
                Order(activate);
            }
        }

        private void Order(bool activate)
        {
            orderAccessPoints = realEndpointMap.Values.OfType<IRpcRemoteAccessPoint>().ToImmutableSortedSet();
            switch (activate)
            {
                case true when realEndpointMap.IsEmpty():
                    service.OnNodeUnactivated(this);
                    break;
                case false when !realEndpointMap.IsEmpty():
                    service.OnNodeActivate(this);
                    break;
            }
        }

        internal void RemoveEndpoint(IEndpoint<RpcAccessIdentify> endpoint)
        {
            lock (this)
            {
                var nodeId = endpoint.UserId;
                var activate = this.realEndpointMap.IsEmpty();
                if (!realEndpointMap.TryGetValue(nodeId.Id, out var accessPoint))
                    return;
                var exist = accessPoint.Endpoint;
                if (!ReferenceEquals(endpoint, exist))
                    return;
                if (realEndpointMap.Remove(nodeId.Id))
                {
                    Order(activate);
                }
            }
        }

        public int CompareTo(RpcRemoteNode other)
        {
            if (ReferenceEquals(this, other)) return 0;
            return ReferenceEquals(null, other) ? 1 : ServerId.CompareTo(other.ServerId);
        }
    }

}
