// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using TnyFramework.Common.Event;
using TnyFramework.Net.Application;
using TnyFramework.Net.Message;
using TnyFramework.Net.Session;
using TnyFramework.Net.Transport.Event;

namespace TnyFramework.Net.Transport;

public interface INetTunnel : ITunnel, ITransport
{
    INetSession NetSession { get; }

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
    /// 会话 Session
    /// </summary>
    /// <param name="session">会话</param>
    /// <returns>返回是否绑定成功</returns>
    bool Bind(INetSession session);

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
    IEventWatch<TunnelActivate> ActivateEvent { get; }

    /// <summary>
    /// 断线事件总线, 可监听到当前 Tunnel 的事件
    /// </summary>
    IEventWatch<TunnelUnactivated> UnactivatedEvent { get; }

    /// <summary>
    /// 关闭事件总线, 可监听到当前 Tunnel 的事件
    /// </summary>
    IEventWatch<TunnelClose> CloseEvent { get; }
}
