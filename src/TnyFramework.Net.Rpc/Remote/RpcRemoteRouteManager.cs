using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace TnyFramework.Net.Rpc.Remote
{

    public class RpcRemoteRouteManager : IRpcRemoteRouteManager
    {
        private readonly IRpcRemoteRouter defaultRemoteRouter;

        private readonly IDictionary<Type, IRpcRemoteRouter> routers;

        public RpcRemoteRouteManager(IRpcRemoteRouter defaultRemoteRouter, IEnumerable<IRpcRemoteRouter> routers)
        {
            this.defaultRemoteRouter = defaultRemoteRouter;
            this.routers = routers.ToImmutableDictionary(
                router => router.GetType(),
                router => router);
        }

        public IRpcRemoteRouter GetRouter<T>() where T : IRpcRemoteRouter
        {
            return GetRouter(typeof(T));
        }

        public IRpcRemoteRouter GetRouter(Type type)
        {
            if (type == null || type == typeof(IRpcRemoteRouter))
            {
                return defaultRemoteRouter;
            }
            return routers.TryGetValue(type, out var router) ? router : null;
        }
    }

}
