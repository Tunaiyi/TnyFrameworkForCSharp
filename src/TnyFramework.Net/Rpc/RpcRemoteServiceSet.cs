using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using TnyFramework.Net.Endpoint;
using TnyFramework.Net.Transport;

namespace TnyFramework.Net.Rpc
{

    public class RpcRemoteServiceSet : IRpcRemoteSet
    {
        private readonly ConcurrentDictionary<int, RpcRemoteServiceNode> remoteNodeMap = new ConcurrentDictionary<int, RpcRemoteServiceNode>();

        private volatile IList<RpcRemoteServiceNode> orderRemoteNodes = ImmutableList.Create<RpcRemoteServiceNode>();

        private volatile int version;

        public IRpcServiceType ServiceType { get; }

        public int Version => version;

        public IList<RpcRemoteServiceNode> OrderRemoteNodes => orderRemoteNodes;

        public RpcRemoteServiceSet(IRpcServiceType serviceType)
        {
            ServiceType = serviceType;
        }

        public IRpcRemoteNode FindRemoteNode(int nodeId)
        {
            return remoteNodeMap[nodeId];
        }

        public IRpcRemoteAccess FindRemoteAccess(int nodeId, long accessId)
        {
            var node = remoteNodeMap[nodeId];
            return node?.GetRemoteAccess(accessId);
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

        internal void OnNodeActivate(RpcRemoteServiceNode rpcServiceNode)
        {
            RefreshNodes(rpcServiceNode);
        }

        internal void OnNodeUnactivated(RpcRemoteServiceNode rpcServiceNode)
        {
            RefreshNodes(rpcServiceNode);
        }

        private RpcRemoteServiceNode LoadOrCreate(ICommunicator<RpcAccessIdentify> endpoint)
        {
            var nodeId = endpoint.UserId;
            return remoteNodeMap.GetOrAdd(nodeId.ServerId, CreateNode);
        }

        private RpcRemoteServiceNode CreateNode(int serverId)
        {
            return new RpcRemoteServiceNode(serverId, this);
        }

        private void RefreshNodes(RpcRemoteServiceNode rpcNode)
        {
            if (!remoteNodeMap.TryGetValue(rpcNode.ServerId, out var currentNode))
                return;
            if (!ReferenceEquals(currentNode, rpcNode))
                return;
            orderRemoteNodes = remoteNodeMap.Values.Where(ActiveNode).ToImmutableSortedSet();
            UpdateVersion();
        }

        private static bool ActiveNode(IRpcRemoteNode node)
        {
            return node.IsActive();
        }
    }

}
