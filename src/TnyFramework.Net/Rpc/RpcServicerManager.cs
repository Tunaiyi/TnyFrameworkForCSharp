// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Collections.Concurrent;
using TnyFramework.Common.Extensions;
using TnyFramework.Net.Base;
using TnyFramework.Net.Endpoint;

namespace TnyFramework.Net.Rpc
{

    public class RpcServicerManager : IRpcInvokeNodeManager
    {
        private readonly ConcurrentDictionary<IMessagerType, RpcServiceSet> serviceSetMap =
            new ConcurrentDictionary<IMessagerType, RpcServiceSet>();

        private readonly ConcurrentDictionary<IMessagerType, IRpcInvokeNodeSet> invokeNodeSetMap =
            new ConcurrentDictionary<IMessagerType, IRpcInvokeNodeSet>();

        public RpcServicerManager()
        {
            EndpointKeeperManager.CreateEventBox.Add(OnCreate);
        }

        public IRpcInvokeNodeSet LoadInvokeNodeSet(IMessagerType serviceType)
        {
            return DoLoadInvokeNodeSet(serviceType, null);
        }

        public IRpcInvokeNodeSet FindInvokeNodeSet(IMessagerType serviceType)
        {
            return serviceSetMap.TryGetValue(serviceType, out var serviceSet) ? serviceSet : null;
        }

        private IRpcInvokeNodeSet DoLoadInvokeNodeSet(IMessagerType messagerType, Action<MessagerNodeSet> createHandler)
        {
            if (messagerType is RpcServiceType serviceType)
            {
                return DoLoadRpcServiceSet(serviceType);
            } else
            {
                return DoLoadRpcServiceSet(messagerType, createHandler);
            }
        }

        private IRpcInvokeNodeSet DoLoadRpcServiceSet(IMessagerType messagerType, Action<MessagerNodeSet> createHandler)
        {
            var nodeSet = invokeNodeSetMap.Get(messagerType);
            if (nodeSet != null)
            {
                return nodeSet;
            }
            var newSet = new MessagerNodeSet(messagerType);
            if (invokeNodeSetMap.PutIfAbsent(messagerType, newSet) == null)
            {
                createHandler?.Invoke(newSet);
            }
            return invokeNodeSetMap.Get(messagerType);
        }

        private RpcServiceSet DoLoadRpcServiceSet(IRpcServiceType serviceType)
        {
            RpcServiceSet Creator(IMessagerType type) => CreateServiceSet(serviceType);
            return serviceSetMap.GetOrAdd(serviceType, Creator);
        }

        private static RpcServiceSet CreateServiceSet(IRpcServiceType serviceType) => new RpcServiceSet(serviceType);

        private void OnCreate(IEndpointKeeper keeper)
        {
            if (!(keeper.MessagerType is IRpcServiceType))
                return;
            keeper.AddEndpointEvent.Add(OnAddEndpoint);
            keeper.RemoveEndpointEvent.Add(OnRemoveEndpoint);
        }

        private void OnRemoveEndpoint(IEndpointKeeper keeper, IEndpoint endpoint)
        {
            if (!(endpoint is IEndpoint<RpcAccessIdentify> point))
                return;
            var servicer = DoLoadRpcServiceSet(point.UserId.ServiceType);
            servicer?.RemoveEndpoint(point);
        }

        private void OnAddEndpoint(IEndpointKeeper keeper, IEndpoint endpoint)
        {
            if (!(endpoint is IEndpoint<RpcAccessIdentify> point))
                return;
            var servicer = DoLoadRpcServiceSet(point.UserId.ServiceType);
            servicer?.AddEndpoint(point);
        }
    }

}
