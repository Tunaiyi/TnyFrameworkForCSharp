// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using TnyFramework.Net.Command.Dispatcher;
using TnyFramework.Net.Command.Dispatcher.Monitor;
using TnyFramework.Net.Command.Tasks;
using TnyFramework.Net.Message;
using TnyFramework.Net.Session;

namespace TnyFramework.Net.Application
{

    public class NetworkContext : INetworkContext
    {
        public IMessageDispatcher MessageDispatcher { get; }

        public ICommandBoxFactory CommandBoxFactory { get; }

        public IMessageFactory MessageFactory { get; }

        public ISessionFactory SessionFactory { get; }

        public IContactFactory ContactFactory { get; }

        public RpcMonitor RpcMonitor { get; }

        public IRpcForwarder? RpcForwarder { get; }

        public NetworkContext()
        {
            MessageDispatcher = null!;
            CommandBoxFactory = null!;
            MessageFactory = null!;
            SessionFactory = null!;
            ContactFactory = null!;
            RpcMonitor = null!;
        }

        public NetworkContext(
            IMessageDispatcher messageDispatcher,
            ICommandBoxFactory commandBoxFactory,
            IMessageFactory messageFactory,
            ISessionFactory sessionFactory,
            IContactFactory contactFactory,
            RpcMonitor rpcMonitor,
            IRpcForwarder? rpcForwarder = null)
        {
            MessageDispatcher = messageDispatcher;
            CommandBoxFactory = commandBoxFactory;
            MessageFactory = messageFactory;
            SessionFactory = sessionFactory;
            ContactFactory = contactFactory;
            RpcMonitor = rpcMonitor;
            RpcForwarder = rpcForwarder;
        }
    }

}
