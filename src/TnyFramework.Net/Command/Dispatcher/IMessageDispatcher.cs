// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using TnyFramework.Common.Event;
using TnyFramework.Net.Message;

namespace TnyFramework.Net.Command.Dispatcher
{

    public interface IMessageDispatcher
    {
        /// <summary>
        /// 派发消息事件, 派发消息事件到相对应的Controller
        /// </summary>
        /// <param name="rpcContext">rpc上下文</param>
        /// <returns>命令</returns>
        ICommand Dispatch(IRpcProviderContext rpcContext);

        /// <summary>
        /// 判断是否可以分发
        /// </summary>
        /// <param name="head">请求头</param>
        /// <returns>可以分发返回ture, 否则返回 false</returns>
        bool IsCanDispatch(IMessageHead head);

        /// <summary>
        /// Command执行事件
        /// </summary>
        IEventBox<CommandExecute> CommandExecuteEvent { get; }

        /// <summary>
        /// Command执行完成事件
        /// </summary>
        IEventBox<CommandDone> CommandDoneEvent { get; }

        /// <summary>
        /// 注册 Controller
        /// </summary>
        /// <param name="controllerExecutor"></param>
        void AddController(object controllerExecutor);
    }

}
