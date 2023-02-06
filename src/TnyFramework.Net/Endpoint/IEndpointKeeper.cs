// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System.Collections.Generic;
using TnyFramework.Common.Event;
using TnyFramework.Net.Base;
using TnyFramework.Net.Command;
using TnyFramework.Net.Endpoint.Event;
using TnyFramework.Net.Transport;

namespace TnyFramework.Net.Endpoint
{

    public interface IEndpointKeeper
    {
        /// <summary>
        /// 获取用户类型
        /// </summary>
        IMessagerType MessagerType { get; }

        /// <summary>
        /// 获取用户组
        /// </summary>
        string UserGroup { get; }

        /// <summary>
        /// 所有endpoint数量
        /// </summary>
        int Size { get; }

        /// <summary>
        /// 获取指定userId对应的Session <br>
        /// </summary>
        /// <param name="userId">指定的Key</param>
        /// <returns>返回获取的endpoint</returns>
        IEndpoint GetEndpoint(object userId);

        /// <summary>
        /// 获取所有的endpoints
        /// </summary>
        /// <returns>返回endpoints</returns>
        IList<IEndpoint> GetAllEndpoints();

        /// <summary>
        /// 使指定userId的endpoint关闭
        /// </summary>
        /// <param name="userId">指定userId</param>
        /// <returns>返回关闭endpoint</returns>
        IEndpoint Close(object userId);

        /// <summary>
        /// 使指定userId的endpoint下线
        /// </summary>
        /// <param name="userId">指定userId</param>
        /// <returns>返回下线的endpoint</returns>
        IEndpoint Offline(object userId);

        /// <summary>
        /// 发信息给用户
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="content"></param>
        void Send2User(object userId, MessageContent content);

        /// <summary>
        /// 发信息给用户集合
        /// </summary>
        /// <param name="userIds">用户ID列表</param>
        /// <param name="content"></param>
        void Send2Users(IEnumerable<object> userIds, MessageContent content);

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
        IEndpoint Online(ICertificate certificate, INetTunnel tunnel);

        /**
        * <p>
        * <p>
        * 获取指定userId对应的Session <br>
        *
        * @param userId 指定的Key
        * @return 返回获取的endpoint, 无endpoint返回null
        */
        bool IsOnline(object userId);

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

    public interface IEndpointKeeper<in TUserId, TEndpoint> : IEndpointKeeper
        where TEndpoint : IEndpoint<TUserId>
    {
        /// <summary>
        /// 获取指定userId对应的Session <br>
        /// </summary>
        /// <param name="userId">指定的Key</param>
        /// <returns>返回获取的endpoint</returns>
        TEndpoint GetEndpoint(TUserId userId);

        /// <summary>
        /// 获取所有的endpoints
        /// </summary>
        /// <returns>返回endpoints</returns>
        new IList<TEndpoint> GetAllEndpoints();

        /// <summary>
        /// 使指定userId的endpoint关闭
        /// </summary>
        /// <param name="userId">指定userId</param>
        /// <returns>返回关闭endpoint</returns>
        TEndpoint Close(TUserId userId);

        /// <summary>
        /// 使指定userId的endpoint下线
        /// </summary>
        /// <param name="userId">指定userId</param>
        /// <returns>返回下线的endpoint</returns>
        TEndpoint Offline(TUserId userId);

        /// <summary>
        /// 判断指定用户是否在线
        /// </summary>
        /// <param name="userId">用户 id</param>
        /// <returns>在线返回 true 否则返回 false</returns>
        bool IsOnline(TUserId userId);
    }

}
