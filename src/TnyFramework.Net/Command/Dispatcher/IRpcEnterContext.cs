// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using TnyFramework.Net.Base;
using TnyFramework.Net.Command.Dispatcher.Monitor;
using TnyFramework.Net.Message;
using TnyFramework.Net.Transport;

namespace TnyFramework.Net.Command.Dispatcher
{

    public interface IRpcEnterContext : IRpcInvocationContext, IRpcTransferContext, IRpcHandleContext
    {
        /// <summary>
        /// 恢复
        /// </summary>
        /// <returns></returns>
        bool Resume();

        /// <summary>
        /// 挂起
        /// </summary>
        /// <returns></returns>
        bool Suspend();

        /// <summary>
        /// 运行中
        /// </summary>
        /// <returns></returns>
        bool Running();

        /// <summary>
        /// 消息
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
    }

}
