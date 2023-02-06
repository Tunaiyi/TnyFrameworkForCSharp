// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using TnyFramework.Common.Attribute;
using TnyFramework.Coroutines.Async;
using TnyFramework.Net.Endpoint;
using TnyFramework.Net.Message;
using TnyFramework.Net.Rpc;

namespace TnyFramework.Net.Command.Dispatcher
{

    public interface IRpcContext
    {
        /// <summary>
        /// 获取消息
        /// </summary>
        ///
        /// <return>获取消息</return>
        IMessageSubject MessageSubject { get; }

        /// <summary>
        /// 请求模式
        /// </summary>
        ///
        /// <return>请求模式</return>
        RpcInvocationMode InvocationMode { get; }

        /// <summary>
        /// 获取终端
        /// </summary>
        ///
        /// <return>获取终端</return>
        IEndpoint GetEndpoint();

        /// <summary>
        /// 当前执行器
        /// </summary>
        ///
        /// <return>当前执行器</return>
        IAsyncExecutor Executor { get; }

        /// <summary>
        /// 附加属性
        /// </summary>
        ///
        /// <return>附加属性</return>
        IAttributes Attributes { get; }

    }

}
