// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;

namespace TnyFramework.Net.Command.Dispatcher
{

    /// <summary>
    /// 开始执行
    /// </summary>
    /// <param name="command">command </param>
    public delegate void CommandExecute(RpcHandleCommand command);

    /// <summary>
    /// 执行Command任务完成
    /// </summary>
    /// <param name="command">command</param>
    /// <param name="cause">异常</param>
    public delegate void CommandDone(RpcHandleCommand command, Exception cause);

    // public interface IMessageCommandListener
    // {
    //     /// <summary>
    //     /// 开始执行
    //     /// </summary>
    //     /// <param name="command">command </param>
    //     void OnExecute(MessageCommand command);
    //
    //
    //     /// <summary>
    //     /// 执行Command任务完成
    //     /// </summary>
    //     /// <param name="command">command</param>
    //     /// <param name="cause">异常</param>
    //     void OnDone(MessageCommand command, Exception cause);
    // }

}
