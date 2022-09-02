using TnyFramework.Net.Endpoint;
using TnyFramework.Net.Message;
using TnyFramework.Net.Transport;

namespace TnyFramework.Net.Rpc
{

    public class RpcEndpointAccessPoint : IRpcRemoteAccessPoint
    {
        public RpcAccessIdentify AccessId => Endpoint.UserId;

        public Message.ForwardPoint ForwardPoint { get; }

        internal IEndpoint<RpcAccessIdentify> Endpoint { get; }

        public RpcEndpointAccessPoint(IEndpoint<RpcAccessIdentify> endpoint)
        {
            Endpoint = endpoint;
            ForwardPoint = new Message.ForwardPoint(AccessId);
        }

        public ISendReceipt Send(MessageContext messageContext)
        {
            return Endpoint.Send(messageContext);
        }

        public int CompareTo(IRpcRemoteAccessPoint other)
        {
            if (ReferenceEquals(this, other)) return 0;
            return ReferenceEquals(null, other) ? 1 : AccessId.CompareTo(other.AccessId);
        }
    }

}
