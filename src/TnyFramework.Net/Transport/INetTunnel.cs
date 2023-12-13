// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using TnyFramework.Common.Event;
using TnyFramework.Net.Base;
using TnyFramework.Net.Command;
using TnyFramework.Net.Command.Dispatcher;
using TnyFramework.Net.Endpoint;
using TnyFramework.Net.Message;
using TnyFramework.Net.Transport.Event;

namespace TnyFramework.Net.Transport
{

    public interface INetTunnel : ITunnel, ITransport, INetMessager, IMessageSender
    {
        /// <summary>
        /// 接收消息
        /// </summary>
        /// <param name="message"></param>
        /// <returns>是否接收成功</returns>
        bool Receive(INetMessage message);

        /// <summary>
        /// 访问 id
        /// </summary>
        /// <returns></returns>
        void SetAccessId(long accessId);

        /// <summary>
        /// Message 工厂
        /// </summary>
        IMessageFactory MessageFactory { get; }

        /// <summary>
        /// 终端
        /// </summary>
        new INetEndpoint GetEndpoint();

        /// <summary>
        /// 认证工厂
        /// </summary>
        ICertificateFactory CertificateFactory { get; }

        /// <summary>
        /// 终端 Endpoint
        /// </summary>
        /// <param name="endpoint">终端</param>
        /// <returns>返回是否绑定成功</returns>
        bool Bind(INetEndpoint endpoint);

        /// <summary>
        /// 打开通道
        /// </summary>
        /// <returns></returns>
        bool Open();

        /// <summary>
        /// 断开连接
        /// </summary>
        /// <returns></returns>
        void Disconnect();

        /// <summary>
        /// 断开并重置状态
        /// </summary>
        void Reset();

        /// <summary>
        /// Pong
        /// </summary>
        void Pong();

        /// <summary>
        /// Ping
        /// </summary>
        void Ping();

        /// <summary>
        /// 获取上下文
        /// </summary>
        /// <returns></returns>
        INetworkContext Context { get; }

        /// <summary>
        /// 激活事件总线, 可监听到当前 Tunnel 的事件
        /// </summary>
        IEventBox<TunnelActivate> ActivateEvent { get; }

        /// <summary>
        /// 断线事件总线, 可监听到当前 Tunnel 的事件
        /// </summary>
        IEventBox<TunnelUnactivated> UnactivatedEvent { get; }

        /// <summary>
        /// 关闭事件总线, 可监听到当前 Tunnel 的事件
        /// </summary>
        IEventBox<TunnelClose> CloseEvent { get; }
    }

    public interface INetTunnel<TUserId> : ITunnel<TUserId>, INetTunnel
    {
        new INetEndpoint<TUserId> Endpoint { get; }

        /// <summary>
        /// 认证工厂
        /// </summary>
        new ICertificateFactory<TUserId> CertificateFactory { get; }
    }

    public static class NetTunnelExtensions
    {
        public static INetTunnel<TUserId> As<TUserId>(this INetTunnel tunnel)
        {
            return (INetTunnel<TUserId>) tunnel;
        }
    }

}
