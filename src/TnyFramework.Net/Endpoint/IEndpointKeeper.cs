// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System.Collections.Generic;
using TnyFramework.Common.Event;
using TnyFramework.Net.Application;
using TnyFramework.Net.Endpoint.Event;
using TnyFramework.Net.Message;
using TnyFramework.Net.Transport;

namespace TnyFramework.Net.Endpoint
{

    public interface IEndpointKeeper
    {
        /// <summary>
        /// 获取用户类型
        /// </summary>
        IContactType ContactType { get; }

        /// <summary>
        /// 所有endpoint数量
        /// </summary>
        int Size { get; }

        /// <summary>
        /// 获取指定identify对应的Session <br />
        /// </summary>
        /// <param name="identify">指定的Key</param>
        /// <returns>返回获取的endpoint</returns>
        IEndpoint? GetEndpoint(long identify);

        /// <summary>
        /// 获取所有的endpoints
        /// </summary>
        /// <returns>返回endpoints</returns>
        IList<IEndpoint> GetAllEndpoints();

        /// <summary>
        /// 使指定identify的endpoint关闭
        /// </summary>
        /// <param name="identify">指定identify</param>
        /// <returns>返回关闭endpoint</returns>
        IEndpoint? Close(long identify);

        /// <summary>
        /// 使指定identify的endpoint下线
        /// </summary>
        /// <param name="identify">指定identify</param>
        /// <returns>返回下线的endpoint</returns>
        IEndpoint? Offline(long identify);

        /// <summary>
        /// 发信息给用户
        /// </summary>
        /// <param name="identify"></param>
        /// <param name="content"></param>
        void Send2User(long identify, MessageContent content);

        /// <summary>
        /// 发信息给用户集合
        /// </summary>
        /// <param name="identifies">用户ID列表</param>
        /// <param name="content"></param>
        void Send2Users(IEnumerable<long> identifies, MessageContent content);

        /// <summary>
        /// 发送给所有在线的用户
        /// </summary>
        /// <param name="content">消息内容</param>
        void Send2AllOnline(MessageContent content);

        /// <summary>
        /// 使所有endpoint下线
        /// </summary>
        void OfflineAll();

        /// <summary>
        /// 是所有endpoint关闭
        /// </summary>
        void CloseAll();

        /// <summary>
        /// 计算在线人数 
        /// </summary>
        int OnlineSize { get; }

        /// <summary>
        /// 添加指定的endpoint
        /// </summary>
        /// <param name="certificate">认证</param>
        /// <param name="tunnel">注册tunnel</param>
        /// <returns>返回注册的 endpoint</returns>
        IEndpoint? Online(ICertificate certificate, INetTunnel tunnel);

        /**
         * 获取指定identify对应的Session
         * @param identify 指定的Key
         * @return 返回获取的endpoint, 无endpoint返回null
         */
        bool IsOnline(long identify);

        /// <summary>
        /// 添加 Endpoint 事件
        /// </summary>
        IEventBox<EndpointKeeperAddEndpoint> AddEndpointEvent { get; }

        /// <summary>
        /// 移出 Endpoint 事件
        /// </summary>
        IEventBox<EndpointKeeperRemoveEndpoint> RemoveEndpointEvent { get; }

        /// <summary>
        /// 启动
        /// </summary>
        void Start();
    }

    public interface IEndpointKeeper<TEndpoint> : IEndpointKeeper
        where TEndpoint : IEndpoint
    {
        /// <summary>
        /// 获取指定identify对应的Session <br />
        /// </summary>
        /// <param name="identify">指定的Key</param>
        /// <returns>返回获取的endpoint</returns>
        new TEndpoint? GetEndpoint(long identify);

        /// <summary>
        /// 获取所有的endpoints
        /// </summary>
        /// <returns>返回endpoints</returns>
        new IList<TEndpoint> GetAllEndpoints();

        /// <summary>
        /// 使指定identify的endpoint关闭
        /// </summary>
        /// <param name="identify">指定identify</param>
        /// <returns>返回关闭endpoint</returns>
        new TEndpoint? Close(long identify);

        /// <summary>
        /// 使指定identify的endpoint下线
        /// </summary>
        /// <param name="identify">指定identify</param>
        /// <returns>返回下线的endpoint</returns>
        new TEndpoint? Offline(long identify);
    }

}
