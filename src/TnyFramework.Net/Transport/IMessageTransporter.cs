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

    public interface IMessageTransporter : IConnection
    {
        /// <summary>
        /// 绑定通道通道
        /// </summary>
        /// <param name="tunnel">通道</param>
        void Bind(INetTunnel tunnel);

        /// <summary>
        /// 发送消
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="waitWritten">是否等待写出</param>
        /// <returns>写出等待对象</returns>
        ValueTask Write(IMessage message, bool waitWritten = false);

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="maker">消息创建器</param>
        /// <param name="factory">消息消息工厂</param>
        /// <param name="content">消息上下文</param>
        /// <param name="waitWritten">是否等待写出</param>
        /// <returns>写出等待对象</returns>
        ValueTask Write(IMessageAllocator maker, IMessageFactory factory, MessageContent content, bool waitWritten = false);

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="maker">消息创建器</param>
        /// <param name="factory">消息消息工厂</param>
        /// <param name="content">消息上下文</param>
        /// <param name="waitWritten">是否等待写出</param>
        /// <returns>写出等待对象</returns>
        ValueTask Write(MessageAllocator maker, IMessageFactory factory, MessageContent content, bool waitWritten = false);
    }

}
