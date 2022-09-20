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
