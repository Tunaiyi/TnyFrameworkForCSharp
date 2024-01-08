// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System.Threading.Tasks;
using TnyFramework.Common.Event;
using TnyFramework.Net.Command.Dispatcher;
using TnyFramework.Net.Command.Tasks;
using TnyFramework.Net.Message;
using TnyFramework.Net.Session.Event;
using TnyFramework.Net.Transport;

namespace TnyFramework.Net.Session
{

    public interface INetSession : ISession
    {
        /// <summary>
        /// 接收消息
        /// </summary>
        /// <param name="rpcMessageContext">消息</param>
        /// <returns>是否接收成功</returns>
        bool Receive(IRpcMessageEnterContext rpcMessageContext);

        /// <summary>
        /// 异步发送消息
        /// </summary>
        /// <param name="sendTunnel">发送的通道</param>
        /// <param name="content">发送消息上下文</param>
        /// <param name="waitWritten"></param>
        /// <returns>返回发送回执</returns>
        ValueTask<IMessageSent> Send(INetTunnel sendTunnel, MessageContent content, bool waitWritten = false);

        /// <summary>
        /// 分配生成消息
        /// </summary>
        /// <param name="messageFactory">消息工厂</param>
        /// <param name="content">发送内容</param>
        /// <returns>返回创建消息</returns>
        internal INetMessage CreateMessage(IMessageFactory messageFactory, MessageContent content);

        /// <summary>
        /// 使用指定认证登陆
        /// </summary>
        /// <param name="newCertificate">指定认证</param>
        /// <param name="onlineOne">通道</param>
        void Online(ICertificate newCertificate);

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
        CommandBox CommandBox { get; }

        /// <summary>
        /// 接管载入消息盒子
        /// </summary>
        /// <param name="commandBox">消息</param>
        void TakeOver(CommandBox commandBox);

        /// <summary>
        /// 终端下文
        /// </summary>
        ISessionContext Context { get; }

        /// <summary>
        /// 上线事件总线, 可监听到当前 Session 的事件
        /// </summary>
        IEventWatch<SessionOnline> OnlineEvent { get; }

        /// <summary>
        /// 下线事件总线, 可监听到当前 Session 的事件
        /// </summary>
        IEventWatch<SessionOffline> OfflineEvent { get; }

        /// <summary>
        /// 关闭事件总线, 可监听到当前 Session 的事件
        /// </summary>
        IEventWatch<SessionClose> CloseEvent { get; }
    }

}
