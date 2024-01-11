// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using TnyFramework.Net.Session;

namespace TnyFramework.Net.Command.Dispatcher;

public interface IRpcHandleContext
{
    /// <summary>
    /// 获取会话
    /// </summary>
    ///
    /// <return>获取会话</return>
    ISession GetSession();

    // /// <summary>
    // /// 当前执行器
    // /// </summary>
    // ///
    // /// <return>当前执行器</return>
    // IAsyncExecutor Executor { get; }
}
