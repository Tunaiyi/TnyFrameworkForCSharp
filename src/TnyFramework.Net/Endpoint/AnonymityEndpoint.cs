using TnyFramework.Net.Command;
using TnyFramework.Net.Transport;
namespace TnyFramework.Net.Endpoint
{
    public class AnonymityEndpoint<TUserId> : NetEndpoint<TUserId>, INetSession<TUserId>
    {
        public AnonymityEndpoint(ICertificateFactory<TUserId> certificateFactory, IEndpointContext context)
            : base(certificateFactory.Anonymous(), context)
        {
        }


        public AnonymityEndpoint(ICertificateFactory<TUserId> certificateFactory, IEndpointContext context, INetTunnel<TUserId> tunnel)
            : base(certificateFactory.Anonymous(), context)
        {
            SetTunnel(tunnel);
        }


        public override void OnUnactivated(INetTunnel tunnel) => Close();
        
    }
}
