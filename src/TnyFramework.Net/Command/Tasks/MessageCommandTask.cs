using TnyFramework.Net.Dispatcher;
using TnyFramework.Net.Message;
using TnyFramework.Net.Rpc;
using TnyFramework.Net.Transport;

namespace TnyFramework.Net.Command.Tasks
{

    public class MessageCommandTask : ICommandTask
    {
        private readonly IMessage message;

        private readonly INetTunnel tunnel;

        private readonly IMessageDispatcher messageDispatcher;


        public MessageCommandTask(IMessage message, INetTunnel tunnel, IMessageDispatcher messageDispatcher)
        {
            this.message = message;
            this.tunnel = tunnel;
            this.messageDispatcher = messageDispatcher;
        }


        public ICommand Command {
            get {
                if (message.ExistHeader(MessageHeaderConstants.RPC_FORWARD_HEADER))
                {
                    if (tunnel is INetTunnel<RpcAccessIdentify> netTunnel)
                    {
                        return new MessageForwardCommand(netTunnel, message);
                    }
                }
                switch (message.Mode)
                {
                    case MessageMode.Push:
                    case MessageMode.Request:
                    case MessageMode.Response:
                        return messageDispatcher.Dispatch(tunnel, message);
                    case MessageMode.Ping:
                        return new RunnableCommand(() => tunnel.Pong());
                    case MessageMode.Pong:
                    default: return null;
                }
            }
        }
    }

}
