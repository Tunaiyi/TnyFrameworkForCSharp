using TnyFramework.Net.Command;
using TnyFramework.Net.Command.Processor;
using TnyFramework.Net.Dispatcher;
using TnyFramework.Net.Message;

namespace TnyFramework.Net.Base
{

    public class NetworkContext<TUserId> : INetworkContext
    {
        public IMessageDispatcher MessageDispatcher { get; }

        public ICommandTaskBoxProcessor CommandTaskProcessor { get; }

        public IMessageFactory MessageFactory { get; }

        public IMessagerFactory MessagerFactory { get; }

        public IServerBootstrapSetting Setting { get; }

        public ICertificateFactory<TUserId> CertificateFactory { get; }

        public IRpcForwarder RpcForwarder { get; }

        public ICertificateFactory GetCertificateFactory() => CertificateFactory;

        public NetworkContext()
        {
        }

        public NetworkContext(
            IServerBootstrapSetting setting,
            IMessageDispatcher messageDispatcher,
            ICommandTaskBoxProcessor commandTaskProcessor,
            IMessageFactory messageFactory,
            IMessagerFactory messagerFactory,
            ICertificateFactory<TUserId> certificateFactory,
            IRpcForwarder rpcForwarder = null)
        {
            Setting = setting;
            MessageDispatcher = messageDispatcher;
            CommandTaskProcessor = commandTaskProcessor;
            MessageFactory = messageFactory;
            MessagerFactory = messagerFactory;
            CertificateFactory = certificateFactory;
            RpcForwarder = rpcForwarder;
        }

        ICertificateFactory<TUid> INetworkContext.CertificateFactory<TUid>()
        {
            return (ICertificateFactory<TUid>) CertificateFactory;
        }
    }

}
