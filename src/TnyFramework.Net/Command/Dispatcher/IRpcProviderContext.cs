// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using TnyFramework.Common.Result;
using TnyFramework.Coroutines.Async;
using TnyFramework.Net.Base;
using TnyFramework.Net.Command.Dispatcher.Monitor;
using TnyFramework.Net.Message;
using TnyFramework.Net.Transport;

namespace TnyFramework.Net.Command.Dispatcher
{

    public interface IRpcProviderContext : IRpcInvocationContext
    {
        /// <summary>
        /// @return 获取消息
        /// </summary>
        INetMessage NetMessage { get; }

        /// <summary>
        /// @return 获取通道
        /// </summary>
        INetTunnel NetTunnel { get; }

        /// <summary>
        /// @return rpc监控
        /// </summary>
        RpcMonitor RpcMonitor { get; }

        /// <summary>
        /// @return rpc监控
        /// </summary>
        INetworkContext NetworkContext { get; }


        /// <summary>
        /// 静默完成
        /// </summary>
        /// <param name="error">错误原因</param>
        /// <return>是否完成成功</return>
        bool CompleteSilently(Exception error = null);

        /// <summary>
        /// 静默完成
        /// </summary>
        /// <param name="code">错误原因</param>
        /// <param name="body">消息体</param>
        /// <return>是否完成成功</return>
        bool CompleteSilently(IResultCode code, object body = null);

        /// <summary>
        /// 完成并响应
        /// </summary>
        /// <param name="code">结果码</param>
        /// <return>是否完成成功</return>
        bool Complete(IResultCode code);

        /// <summary>
        /// 完成并响应
        /// </summary>
        /// <param name="code">结果码</param>
        /// <return>是否完成成功</return>
        bool Complete(IResultCode code, object body, Exception error = null);

        /// <summary>
        /// 完成并响应
        /// </summary>
        /// <param name="content">响应消息</param>
        /// <param name="error">错误原因</param>
        /// <return>是否完成成功</return>
        bool Complete(MessageContent content, Exception error);
    }

}
