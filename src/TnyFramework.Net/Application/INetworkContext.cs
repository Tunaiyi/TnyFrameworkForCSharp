// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using TnyFramework.Net.Command.Dispatcher.Monitor;
using TnyFramework.Net.Message;
using TnyFramework.Net.Rpc;
using TnyFramework.Net.Session;

namespace TnyFramework.Net.Application;

public interface INetworkContext : ISessionContext, IRpcContext
{
    /// <summary>
    /// 消息工厂
    /// </summary>
    IMessageFactory MessageFactory { get; }

    /// <summary>
    /// 会话工厂
    /// </summary>
    ISessionFactory SessionFactory { get; }

    /// <summary>
    /// 消息者工厂
    /// </summary>
    IContactFactory ContactFactory { get; }

    /// <summary>
    /// 服务配置¬
    /// </summary>
    RpcMonitor RpcMonitor { get; }
}
