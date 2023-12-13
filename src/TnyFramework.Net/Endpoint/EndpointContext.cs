// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using TnyFramework.Net.Command.Dispatcher;
using TnyFramework.Net.Command.Tasks;

namespace TnyFramework.Net.Endpoint
{

    /// <summary>
    /// 终端上下文
    /// </summary>
    public interface IEndpointContext
    {
        /// <summary>
        /// 消息分发器
        /// </summary>
        IMessageDispatcher MessageDispatcher { get; }

        /// <summary>
        /// 命令任务
        /// </summary>
        ICommandBoxFactory CommandBoxFactory { get; }
    }

}
