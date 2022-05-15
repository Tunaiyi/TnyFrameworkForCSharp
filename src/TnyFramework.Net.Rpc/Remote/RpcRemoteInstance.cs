using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;

namespace TnyFramework.Net.Rpc.Remote
{

    public class RpcRemoteInstance
    {
        internal Type RpcType { get; }

        internal RpcRemoteSetting Setting { get; }

        internal RpcRemoteServiceSet ServiceSet { get; }

        private IDictionary<MethodInfo, IRpcRemoteInvoker> invokerMap = ImmutableDictionary<MethodInfo, IRpcRemoteInvoker>.Empty;

        public RpcRemoteInstance(Type rpcType, RpcRemoteSetting setting, RpcRemoteServiceSet serviceSet)
        {
            RpcType = rpcType;
            Setting = setting;
            ServiceSet = serviceSet;
        }

        internal IDictionary<MethodInfo, IRpcRemoteInvoker> InvokerMap {
            get => invokerMap;
            set => invokerMap = value.ToImmutableDictionary();
        }

        public IRpcRemoteInvoker Invoker(MethodInfo method)
        {
            return invokerMap.TryGetValue(method, out var invoker) ? invoker : null;
        }
    }

}
