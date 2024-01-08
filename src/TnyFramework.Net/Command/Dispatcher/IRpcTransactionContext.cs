// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using TnyFramework.Net.Rpc;
using TnyFramework.Net.Transport;

namespace TnyFramework.Net.Command.Dispatcher
{

    public interface IRpcTransactionContext : IRpcMessageContext
    {
        /// <summary>
        /// 请求模式
        /// </summary>
        /// <return>请求模式</return>
        RpcTransactionMode Mode { get; }

        /// <summary>
        /// 消息者
        /// </summary>
        ICommunicator Communicator { get; }

        /// <summary>
        /// @return 获取消息
        /// </summary>
        bool Completed { get; }

        /// <summary>
        /// 成功并响应
        /// </summary>
        /// <return>是否完成成功</return>
        bool Complete();

        /// <summary>
        /// @return 操作名
        /// </summary>
        string? OperationName { get; }

        /// <summary>
        /// 失败并响应
        /// </summary>
        /// <param name="error">错误原因</param>
        /// <return>是否完成成功</return>
        bool Complete(Exception error);

        /// <summary>
        /// 获取错误原因
        /// </summary>
        /// <return>获取错误原因</return>
        Exception? Cause { get; }

        /// <summary>
        /// 是否异步
        /// </summary>
        /// <returns></returns>
        bool Async { get; }

        /// <summary>
        /// 获取错误原因
        /// </summary>
        /// <return>是否错误(异常)</return>
        bool IsError();
    }

}
