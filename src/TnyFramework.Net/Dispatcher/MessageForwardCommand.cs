using System.Threading.Tasks;
using TnyFramework.Net.Message;
using TnyFramework.Net.Rpc;
using TnyFramework.Net.Transport;

namespace TnyFramework.Net.Dispatcher
{

    public class MessageForwardCommand : BaseCommand
    {
        private IMessage message;

        private IRpcContext rpcContext;

        private INetTunnel<RpcAccessIdentify> tunnel;

        public MessageForwardCommand(INetTunnel<RpcAccessIdentify> tunnel, IMessage message)
        {
            this.tunnel = tunnel;
            this.message = message;
            rpcContext = tunnel.Context;
        }

        protected override Task Action()
        {
            //TODO 实现转发逻辑
            return Task.CompletedTask;
        }
    }

}
