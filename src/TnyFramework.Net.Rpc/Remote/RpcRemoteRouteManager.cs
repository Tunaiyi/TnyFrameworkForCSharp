// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

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
