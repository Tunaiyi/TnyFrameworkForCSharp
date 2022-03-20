using TnyFramework.Net.Command;
using TnyFramework.Net.Rpc;
using TnyFramework.Net.Transport;
namespace TnyFramework.Net.Endpoint
{
    
    public class Session<TUserId> : NetEndpoint<TUserId>, INetSession<TUserId>
    {
        public Session(ICertificate<TUserId> certificate, IEndpointContext context) : base(certificate, context)
        {
        }


        public override void OnUnactivated(INetTunnel tunnel)
        {
            if (IsOffline())
            {
                return;
            }
            lock (this)
            {
                if (IsOffline())
                {
                    return;
                }
                var current = CurrentTunnel;
                if (current.IsActive())
                {
                    return;
                }
                if (IsClosed())
                {
                    return;
                }
                SetOffline();
            }
        }


        public override string ToString()
        {
            return $"Session[UserType:{UserType}, UserId:${UserId}, Tunnel:{CurrentTunnel}]";
        }
    }
}
