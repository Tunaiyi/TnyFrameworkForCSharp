// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System.Threading.Tasks;
using TnyFramework.Net.Endpoint;
using TnyFramework.Net.Message;

namespace TnyFramework.Net.Transport
{

    public interface ITransport
    {
        /// <summary>
        /// 写出消息
        /// </summary>
        /// <param name="message">发送消息</param>
        /// <returns>发送promise</returns>
        Task Write(IMessage message);

        /// <summary>
        /// 写出消息
        /// </summary>
        /// <param name="allocator">消费分发器</param>
        /// <param name="messageContext">消息上下文</param>
        /// <returns></returns>
        Task Write(MessageAllocator allocator, MessageContext messageContext);

        /// <summary>
        /// 写出消息
        /// </summary>
        /// <param name="allocator">消费分发器</param>
        /// <param name="messageContext">消息上下文</param>
        /// <returns></returns>
        Task Write(IMessageAllocator allocator, MessageContext messageContext);
    }

}
