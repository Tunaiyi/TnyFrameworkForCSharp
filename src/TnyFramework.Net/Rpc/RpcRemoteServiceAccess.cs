using TnyFramework.Net.Endpoint;
using TnyFramework.Net.Transport;

namespace TnyFramework.Net.Rpc
{

    public class RpcRemoteServiceAccess : IRpcServiceAccess
    {
        public IEndpoint Endpoint { get; }

        public long AccessId => Endpoint.MessagerId;

        public RpcRemoteServiceAccess(IEndpoint endpoint)
        {
            Endpoint = endpoint;
        }

        public ISendReceipt Send(MessageContext messageContext)
        {
            return Endpoint.Send(messageContext);
        }

        public bool IsActive()
        {
            return Endpoint.IsActive();
        }
    }

}
