using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace TnyFramework.Net.Rpc.Remote
{

    public class RpcRemoteRouteManager : IRpcRemoteRouteManager
    {
        private readonly IRpcRouter defaultRouter;

        private readonly IDictionary<Type, IRpcRouter> routers;

        public RpcRemoteRouteManager(IRpcRouter defaultRouter, IEnumerable<IRpcRouter> routers)
        {
            this.defaultRouter = defaultRouter;
            this.routers = routers.ToImmutableDictionary(
                router => router.GetType(),
                router => router);
        }

        public IRpcRouter GetRouter<T>() where T : IRpcRouter
        {
            return GetRouter(typeof(T));
        }

        public IRpcRouter GetRouter(Type type)
        {
            if (type == null || type == typeof(IRpcRouter))
            {
                return defaultRouter;
            }
            return routers.TryGetValue(type, out var router) ? router : null;
        }
    }

}
