// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using TnyFramework.Common.Extensions;
using TnyFramework.Common.Logger;
using TnyFramework.Net.Attributes;
using TnyFramework.Net.Message;

namespace TnyFramework.Net.Command.Dispatcher
{

    public class RpcProfile
    {
        private static readonly ILogger LOGGER = LogFactory.Logger<RpcProfile>();

        public int Protocol { get; }

        public int Line { get; }

        public MessageMode Mode { get; }

        public RpcProfile(RpcProtocolAttribute attribute, MessageMode mode)
        {
            Protocol = attribute.Protocol;
            Line = attribute.Line;
            Mode = mode;
        }

        public static RpcProfile? OneOf(MethodInfo method)
        {
            var rpcRequest = method.GetCustomAttribute<RpcRequestAttribute>();
            if (rpcRequest != null)
            {
                return new RpcProfile(rpcRequest, MessageMode.Request);
            }

            var rpcPush = method.GetCustomAttribute<RpcPushAttribute>();
            if (rpcPush != null)
            {
                return new RpcProfile(rpcPush, MessageMode.Push);
            }

            var rpcResponse = method.GetCustomAttribute<RpcResponseAttribute>();
            if (rpcResponse != null)
            {
                return new RpcProfile(rpcResponse, MessageMode.Response);
            }
            LOGGER.LogWarning("{Method} 没有存在注解 {RpcRequest}, {RpcPush}, {RpcResponse} 中的一个",
                method, typeof(RpcRequestAttribute), typeof(RpcPushAttribute), typeof(RpcResponseAttribute));
            return null;
        }

        public static IList<RpcProfile> AllOf(MethodInfo method, params MessageMode[] defaultModes)
        {
            var profiles = new List<RpcProfile>();
            var rpcRequest = method.GetCustomAttribute<RpcRequestAttribute>();
            if (rpcRequest != null)
            {
                profiles.Add(new RpcProfile(rpcRequest, MessageMode.Request));
            }

            var rpcPush = method.GetCustomAttribute<RpcPushAttribute>();
            if (rpcPush != null)
            {
                profiles.Add(new RpcProfile(rpcPush, MessageMode.Push));
            }

            var rpcResponse = method.GetCustomAttribute<RpcResponseAttribute>();
            if (rpcResponse != null)
            {
                profiles.Add(new RpcProfile(rpcResponse, MessageMode.Response));
            }

            var rpc = method.GetCustomAttribute<RpcAttribute>();
            if (rpc != null)
            {
                if (rpc.MessageModes.Length > 0)
                {
                    profiles.AddRange(rpc.MessageModes.Select(mode => new RpcProfile(rpc, mode)));
                } else
                {
                    profiles.AddRange(defaultModes.Select(mode => new RpcProfile(rpc, mode)));
                }
            }
            if (profiles.IsEmpty())
            {
                LOGGER.LogWarning("{Method} 没有存在注解 {RpcRequest}, {RpcPush}, {RpcResponse}, {Rpc} 中的一个",
                    method, typeof(RpcRequestAttribute), typeof(RpcPushAttribute), typeof(RpcResponseAttribute), typeof(RpcAttribute));
            }
            return profiles;
        }
    }

}
