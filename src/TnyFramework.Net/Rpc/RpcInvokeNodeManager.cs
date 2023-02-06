// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System.Collections.Concurrent;
using TnyFramework.Net.Base;
using TnyFramework.Net.Endpoint;

namespace TnyFramework.Net.Rpc
{

    public class RpcInvokeNodeManager : IRpcInvokeNodeManager
    {
        private readonly ConcurrentDictionary<IRpcServiceType, RpcServiceSet> servicerMap =
            new ConcurrentDictionary<IRpcServiceType, RpcServiceSet>();

        public RpcInvokeNodeManager()
        {
            EndpointKeeperManager.CreateEventBox.Add(OnCreate);
        }

        public RpcServiceSet LoadOrCreate(IRpcServiceType serviceType)
        {
            return servicerMap.GetOrAdd(serviceType, CreateServiceSet);
        }

        public RpcServiceSet Find(IRpcServiceType serviceType)
        {
            return servicerMap.TryGetValue(serviceType, out var serviceSet) ? serviceSet : null;
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
            var servicer = LoadOrCreate(point.UserId.ServiceType);
            servicer?.RemoveEndpoint(point);
        }

        private void OnAddEndpoint(IEndpointKeeper keeper, IEndpoint endpoint)
        {
            if (!(endpoint is IEndpoint<RpcAccessIdentify> point))
                return;
            var servicer = LoadOrCreate(point.UserId.ServiceType);
            servicer?.AddEndpoint(point);
        }

        public IRpcInvokeNodeSet LoadOrCreate(IMessagerType serviceType)
        {
            throw new System.NotImplementedException();
        }

        public IRpcInvokeNodeSet Find(IMessagerType serviceType)
        {
            throw new System.NotImplementedException();
        }
    }

}
