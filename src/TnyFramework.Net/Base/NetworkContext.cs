// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using TnyFramework.Net.Command;
using TnyFramework.Net.Command.Dispatcher;
using TnyFramework.Net.Command.Dispatcher.Monitor;
using TnyFramework.Net.Command.Processor;
using TnyFramework.Net.Message;
using TnyFramework.Net.Transport;

namespace TnyFramework.Net.Base
{

    public class NetworkContext<TUserId> : INetworkContext
    {
        public IMessageDispatcher MessageDispatcher { get; }

        public ICommandTaskBoxProcessor CommandTaskProcessor { get; }

        public IMessageFactory MessageFactory { get; }

        public IContactFactory ContactFactory { get; }

        public IServerBootstrapSetting Setting { get; }

        public RpcMonitor RpcMonitor { get; }

        public ICertificateFactory<TUserId> CertificateFactory { get; }

        public IRpcForwarder? RpcForwarder { get; }

        public ICertificateFactory GetCertificateFactory() => CertificateFactory;

        public NetworkContext()
        {
            MessageDispatcher = null!;
            CommandTaskProcessor = null!;
            MessageFactory = null!;
            ContactFactory = null!;
            Setting = null!;
            RpcMonitor = null!;
            CertificateFactory = null!;
        }

        public NetworkContext(
            IServerBootstrapSetting setting,
            IMessageDispatcher messageDispatcher,
            ICommandTaskBoxProcessor commandTaskProcessor,
            IMessageFactory messageFactory,
            IContactFactory contactFactory,
            ICertificateFactory<TUserId> certificateFactory,
            RpcMonitor rpcMonitor,
            IRpcForwarder? rpcForwarder = null)
        {
            Setting = setting;
            MessageDispatcher = messageDispatcher;
            CommandTaskProcessor = commandTaskProcessor;
            MessageFactory = messageFactory;
            ContactFactory = contactFactory;
            CertificateFactory = certificateFactory;
            RpcMonitor = rpcMonitor;
            RpcForwarder = rpcForwarder;
        }

        ICertificateFactory<TUid> INetworkContext.CertificateFactory<TUid>()
        {
            return (ICertificateFactory<TUid>) CertificateFactory;
        }
    }

}
