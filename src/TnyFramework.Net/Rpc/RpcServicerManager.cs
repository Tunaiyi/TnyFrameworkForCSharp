// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using TnyFramework.Common.Extensions;
using TnyFramework.Net.Application;
using TnyFramework.Net.Endpoint;
using TnyFramework.Net.Rpc.Extensions;

namespace TnyFramework.Net.Rpc
{

    public class RpcServicerManager : IRpcInvokeNodeManager
    {
        private readonly ConcurrentDictionary<IContactType, RpcServiceSet> serviceSetMap = new();

        private readonly ConcurrentDictionary<IContactType, IRpcInvokeNodeSet?> invokeNodeSetMap = new();

        public RpcServicerManager()
        {
            EndpointKeeperManager.CreateEventBox.Add(OnCreate);
        }

        public IRpcInvokeNodeSet? LoadInvokeNodeSet(IContactType serviceType)
        {
            return DoLoadInvokeNodeSet(serviceType, null);
        }

        public IRpcInvokeNodeSet? FindInvokeNodeSet(IContactType serviceType)
        {
            return serviceSetMap.GetValueOrDefault(serviceType);
        }

        private IRpcInvokeNodeSet? DoLoadInvokeNodeSet(IContactType contactType, Action<ContactNodeSet>? createHandler)
        {
            if (contactType is RpcServiceType serviceType)
            {
                return DoLoadRpcServiceSet(serviceType);
            } else
            {
                return DoLoadRpcServiceSet(contactType, createHandler);
            }
        }

        private IRpcInvokeNodeSet? DoLoadRpcServiceSet(IContactType contactType, Action<ContactNodeSet>? createHandler)
        {
            var nodeSet = invokeNodeSetMap.Get(contactType);
            if (nodeSet != null)
            {
                return nodeSet;
            }
            var newSet = new ContactNodeSet(contactType);
            if (invokeNodeSetMap.PutIfAbsent(contactType, newSet) == null)
            {
                createHandler?.Invoke(newSet);
            }
            return invokeNodeSetMap.Get(contactType);
        }

        private RpcServiceSet DoLoadRpcServiceSet(IRpcServiceType serviceType)
        {
            RpcServiceSet Creator(IContactType type) => CreateServiceSet(serviceType);
            return serviceSetMap.GetOrAdd(serviceType, Creator);
        }

        private static RpcServiceSet CreateServiceSet(IRpcServiceType serviceType) => new(serviceType);

        private void OnCreate(IEndpointKeeper keeper)
        {
            if (keeper.ContactType is not IRpcServiceType)
                return;
            keeper.AddEndpointEvent.Add(OnAddEndpoint);
            keeper.RemoveEndpointEvent.Add(OnRemoveEndpoint);
        }

        private void OnRemoveEndpoint(IEndpointKeeper keeper, IEndpoint endpoint)
        {
            var rpcIdentify = endpoint.GetRpcAccessIdentify();
            if (rpcIdentify.IsNull())
            {
                return;
            }
            var servicer = DoLoadRpcServiceSet(rpcIdentify.ServiceType);
            servicer.RemoveEndpoint(endpoint);
        }

        private void OnAddEndpoint(IEndpointKeeper keeper, IEndpoint endpoint)
        {
            var rpcIdentify = endpoint.GetRpcAccessIdentify();
            if (rpcIdentify.IsNull())
            {
                return;
            }
            var servicer = DoLoadRpcServiceSet(rpcIdentify.ServiceType);
            servicer.AddEndpoint(endpoint);
        }
    }

}
