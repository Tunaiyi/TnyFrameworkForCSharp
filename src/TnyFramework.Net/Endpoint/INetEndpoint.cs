// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using TnyFramework.Common.Event;
using TnyFramework.Net.Command;
using TnyFramework.Net.Command.Dispatcher;
using TnyFramework.Net.Command.Tasks;
using TnyFramework.Net.Endpoint.Event;
using TnyFramework.Net.Message;
using TnyFramework.Net.Transport;

namespace TnyFramework.Net.Endpoint
{

    /// <summary>
    /// 终端会话
    /// </summary>
    public interface INetEndpoint : IEndpoint, IMessageReceiver
    {
        // /// <summary>
        // /// 处理收到消息
        // /// </summary>
        // /// <param name="receiver">通道</param>
        // /// <param name="message">消息</param>
        // /// <returns></returns>
        // bool Receive(INetTunnel receiver, IMessage message);

        /// <summary>
        /// 异步发送消息
        /// </summary>
        /// <param name="sendTunnel">发送的通道</param>
        /// <param name="content">发送消息上下文</param>
        /// <returns>返回发送回执</returns>
        ISendReceipt Send(INetTunnel sendTunnel, MessageContent content);

        /// <summary>
        /// 分配生成消息
        /// </summary>
        /// <param name="messageFactory">消息工厂</param>
        /// <param name="content">发送内容</param>
        /// <returns>返回创建消息</returns>
        INetMessage CreateMessage(IMessageFactory messageFactory, MessageContent content);

        /// <summary>
        /// 使用指定认证登陆
        /// </summary>
        /// <param name="newCertificate">指定认证</param>
        /// <param name="onlineOne">通道</param>
        void Online(ICertificate newCertificate, INetTunnel onlineOne);

        /// <summary>
        /// 通知通道销毁
        /// </summary>
        /// <param name="tunnel">销毁通道</param>
        void OnUnactivated(INetTunnel tunnel);

        /// <summary>
        /// 消息盒
        /// </summary>
        CommandTaskBox CommandTaskBox { get; }

        /// <summary>
        /// 接管载入消息盒子
        /// </summary>
        /// <param name="commandTaskBox">消息</param>
        void TakeOver(CommandTaskBox commandTaskBox);

        /// <summary>
        /// 终端下文
        /// </summary>
        IEndpointContext Context { get; }

        /// <summary>
        /// 上线事件总线, 可监听到当前 Endpoint 的事件
        /// </summary>
        IEventBox<EndpointOnline> OnlineEvent { get; }

        /// <summary>
        /// 下线事件总线, 可监听到当前 Endpoint 的事件
        /// </summary>
        IEventBox<EndpointOffline> OfflineEvent { get; }

        /// <summary>
        /// 关闭事件总线, 可监听到当前 Endpoint 的事件
        /// </summary>
        IEventBox<EndpointClose> CloseEvent { get; }
    }

    /// <summary>
    /// 终端会话
    /// </summary>
    /// <typeparam name="TUserId"></typeparam>
    public interface INetEndpoint<out TUserId> : IEndpoint<TUserId>, INetEndpoint
    {
    }

}
