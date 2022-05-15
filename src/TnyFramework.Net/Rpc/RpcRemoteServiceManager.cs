using System.Collections.Concurrent;
using TnyFramework.Net.Endpoint;

namespace TnyFramework.Net.Rpc
{

    public class RpcRemoteServiceManager : IRpcRemoteServiceManager
    {
        private readonly ConcurrentDictionary<IRpcServiceType, RpcRemoteServiceSet> servicerMap =
            new ConcurrentDictionary<IRpcServiceType, RpcRemoteServiceSet>();

        public RpcRemoteServiceManager()
        {
            EndpointKeeperManager.CreateEventBox.Add(OnCreate);
        }

        public RpcRemoteServiceSet LoadOrCreate(IRpcServiceType serviceType)
        {
            return servicerMap.GetOrAdd(serviceType, CreateServiceSet);
        }

        public RpcRemoteServiceSet Find(IRpcServiceType serviceType)
        {
            return servicerMap.TryGetValue(serviceType, out var serviceSet) ? serviceSet : null;
        }

        private static RpcRemoteServiceSet CreateServiceSet(IRpcServiceType serviceType) => new RpcRemoteServiceSet(serviceType);

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
    }

}
