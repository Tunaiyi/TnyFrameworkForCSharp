// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Collections.Generic;
using System.Reflection;
using Castle.DynamicProxy;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using TnyFramework.Common.Logger;
using TnyFramework.Common.Result;
using TnyFramework.Net.Common;
using TnyFramework.Net.Rpc.Attributes;
using TnyFramework.Net.Rpc.Exceptions;

namespace TnyFramework.Net.Rpc.Remote
{

    public class RpcInvokeHandler : IInterceptor
    {
        private readonly RpcRemoteInstance instance;

        public RpcInvokeHandler(RpcRemoteInstance instance)
        {
            this.instance = instance;
        }

        public void Intercept(IInvocation invocation)
        {

            var invoker = instance.Invoker(invocation.Method);
            var arguments = invocation.Arguments;
            invocation.ReturnValue = invoker.Invoke(arguments);
        }
    }

    public class RpcRemoteInstanceFactory
    {
        private static readonly ILogger LOGGER = LogFactory.Logger<RpcInvokeHandler>();

        private readonly RpcRemoteSetting setting;

        private readonly IRpcRemoteServiceManager rpcRemoteService;

        private readonly IRpcRemoteRouteManager rpcRouteManager;

        public RpcRemoteInstanceFactory(RpcRemoteSetting setting, IRpcRemoteServiceManager rpcRemoteService, IRpcRemoteRouteManager rpcRouteManager)
        {
            this.setting = setting;
            this.rpcRemoteService = rpcRemoteService;
            this.rpcRouteManager = rpcRouteManager;
        }

        public T Create<T>()
        {
            return (T) Create(typeof(T));
        }

        public object Create(Type rpcType)
        {
            var factory = new ProxyGenerator();
            var instance = CreateRpcInstance(rpcType);
            object proxy;
            try
            {
                proxy = factory.CreateInterfaceProxyWithoutTarget(rpcType, new RpcInvokeHandler(instance));
            } catch (Exception e)
            {
                throw new RpcException(ResultCode.FAILURE, e, $"create {rpcType} proxy object exception");
            }
            return proxy;
        }

        private RpcRemoteInstance CreateRpcInstance(Type rpcType)
        {
            var methods = RpcRemoteMethod.MethodsOf(rpcType);
            var rpcService = rpcType.GetCustomAttribute<RpcRemoteServiceAttribute>();
            var service = rpcService.Service;
            if (!rpcService.ForwardService.IsNullOrEmpty())
            {
                service = rpcService.ForwardService;
            }
            var serviceType = RpcServiceType.ForService(service);
            var remoteServicer = rpcRemoteService.LoadOrCreate(serviceType);
            var instance = new RpcRemoteInstance(rpcType, setting, remoteServicer);

            IDictionary<MethodInfo, IRpcRemoteInvoker> invokerMap = new Dictionary<MethodInfo, IRpcRemoteInvoker>();
            var count = 0;
            var objectType = typeof(object);
            foreach (var method in methods)
            {
                var router = rpcRouteManager.GetRouter(method.RouterType);
                if (method.Method.DeclaringType == objectType)
                {
                    continue;
                }
                if (router == null)
                {
                    throw new RpcInvokeException(NetResultCode.REMOTE_EXCEPTION, $"?????? {method.Method} ??????, ????????? {method.RouterType} RpcRouter");
                }
                var invoker = new RpcRemoteInvoker(method, instance, router);
                count++;
                invokerMap.Add(method.Method, invoker);
            }
            LOGGER.LogDebug("?????? {Rpc} RpcRemoteInstance ?????? {Count} ?????????", rpcType, count);
            instance.InvokerMap = invokerMap;
            return instance;
        }
    }

}
