using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using TnyFramework.Net.Endpoint;
using TnyFramework.Net.Transport;

namespace TnyFramework.Net.Rpc
{

    public class RpcRemoteServiceSet
    {
        private readonly IRpcServiceType serviceType;

        private readonly ConcurrentDictionary<int, RpcRemoteNode> remoteNodeMap = new ConcurrentDictionary<int, RpcRemoteNode>();

        private volatile IList<RpcRemoteNode> orderRemoteNodes = ImmutableList.Create<RpcRemoteNode>();

        private volatile int version;

        public int Version => version;

        public IList<RpcRemoteNode> OrderRemoteNodes => orderRemoteNodes;

        public RpcRemoteServiceSet(IRpcServiceType serviceType)
        {
            this.serviceType = serviceType;
        }

        private void UpdateVersion()
        {
            Interlocked.Increment(ref version);
        }

        internal void AddEndpoint(IEndpoint<RpcAccessIdentify> endpoint)
        {
            var node = LoadOrCreate(endpoint);
            node.AddEndpoint(endpoint);
            RefreshNodes(node);
        }

        internal void RemoveEndpoint(IEndpoint<RpcAccessIdentify> endpoint)
        {
            var node = LoadOrCreate(endpoint);
            node.RemoveEndpoint(endpoint);
            RefreshNodes(node);
        }

        internal void OnNodeActivate(RpcRemoteNode rpcNode)
        {
            RefreshNodes(rpcNode);
        }

        internal void OnNodeUnactivated(RpcRemoteNode rpcNode)
        {
            RefreshNodes(rpcNode);
        }

        private RpcRemoteNode LoadOrCreate(ICommunicator<RpcAccessIdentify> endpoint)
        {
            var nodeId = endpoint.UserId;
            return remoteNodeMap.GetOrAdd(nodeId.ServerId, CreateNode);
        }

        private RpcRemoteNode CreateNode(int serverId)
        {
            return new RpcRemoteNode(serverId, this);
        }

        private void RefreshNodes(RpcRemoteNode rpcNode)
        {
            if (!remoteNodeMap.TryGetValue(rpcNode.ServerId, out var currentNode))
                return;
            if (!ReferenceEquals(currentNode, rpcNode))
                return;
            orderRemoteNodes = remoteNodeMap.Values.Where(ActiveNode).ToImmutableSortedSet();
            UpdateVersion();
        }

        private static bool ActiveNode(RpcRemoteNode node)
        {
            return node.IsActive();
        }
    }

}
