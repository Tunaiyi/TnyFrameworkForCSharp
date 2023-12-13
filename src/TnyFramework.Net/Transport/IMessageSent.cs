// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System.Threading.Tasks;
using TnyFramework.Net.Message;

namespace TnyFramework.Net.Transport
{

    public interface IMessageSent
    {
        //
        /// <summary>
        /// 获取响应 Task
        /// </summary>
        /// <returns>响应 Task</returns>
        bool Respond(out Task<IMessage> task);

        /// <summary>
        /// 是否写出，如果未写出返回false
        /// </summary>
        /// <returns></returns>
        bool Written { get; }

        // /// <summary>
        // /// 是否可以等待响应
        // /// </summary>
        // /// <returns>是否可以等待响应</returns>
        // bool IsRespondAwaitable();
    }

}
