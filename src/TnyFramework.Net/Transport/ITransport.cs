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
        /// <param name="waitWritten"></param>
        /// <returns>发送promise</returns>
        ValueTask Write(IMessage message, bool waitWritten = false);

        /// <summary>
        /// 写出消息
        /// </summary>
        /// <param name="allocator">消费分发器</param>
        /// <param name="messageContent">消息上下文</param>
        /// <param name="waitWritten"></param>
        /// <returns></returns>
        ValueTask Write(MessageAllocator allocator, MessageContent messageContent, bool waitWritten = false);

        /// <summary>
        /// 写出消息
        /// </summary>
        /// <param name="allocator">消费分发器</param>
        /// <param name="messageContent">消息上下文</param>
        /// <param name="waitWritten"></param>
        /// <returns></returns>
        ValueTask Write(IMessageAllocator allocator, MessageContent messageContent, bool waitWritten = false) =>
            Write(allocator.Allocate, messageContent, waitWritten);
    }

}
