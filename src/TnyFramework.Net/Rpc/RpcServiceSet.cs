// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using TnyFramework.Net.Endpoint;
using TnyFramework.Net.Transport;

namespace TnyFramework.Net.Rpc
{

    public class RpcServiceSet : IRpcInvokeNodeSet
    {
        private readonly ConcurrentDictionary<int, RpcServiceNode> remoteNodeMap = new ConcurrentDictionary<int, RpcServiceNode>();

        private volatile IList<IRpcInvokeNode> orderRemoteNodes = ImmutableList.Create<IRpcInvokeNode>();

        private volatile int version;

        public IRpcServiceType ServiceType { get; }

        public int Version => version;

        public IList<IRpcInvokeNode> OrderRemoteNodes => orderRemoteNodes;

        public RpcServiceSet(IRpcServiceType serviceType)
        {
            ServiceType = serviceType;
        }


        public IList<IRpcInvokeNode> GetOrderInvokeNodes()
        {
            return orderRemoteNodes;
        }

        public IRpcInvokeNode? FindInvokeNode(int nodeId)
        {
            return remoteNodeMap[nodeId];
        }

        public IRpcAccess? FindInvokeAccess(int nodeId, long accessId)
        {
            var node = remoteNodeMap[nodeId];
            return node?.GetAccess(accessId);
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

        internal void OnNodeActivate(RpcServiceNode rpcServiceNode)
        {
            RefreshNodes(rpcServiceNode);
        }

        internal void OnNodeUnactivated(RpcServiceNode rpcServiceNode)
        {
            RefreshNodes(rpcServiceNode);
        }

        private RpcServiceNode LoadOrCreate(ICommunicator<RpcAccessIdentify> endpoint)
        {
            var nodeId = endpoint.UserId;
            return remoteNodeMap.GetOrAdd(nodeId.ServerId, CreateNode);
        }

        private RpcServiceNode CreateNode(int serverId)
        {
            return new RpcServiceNode(serverId, this);
        }

        private void RefreshNodes(RpcServiceNode rpcNode)
        {
            if (!remoteNodeMap.TryGetValue(rpcNode.ServerId, out var currentNode))
                return;
            if (!ReferenceEquals(currentNode, rpcNode))
                return;
            orderRemoteNodes = remoteNodeMap.Values.Where(ActiveNode).Cast<IRpcInvokeNode>().ToImmutableSortedSet();
            UpdateVersion();
        }

        private static bool ActiveNode(IRpcInvokeNode node)
        {
            return node.IsActive();
        }

    }

}
