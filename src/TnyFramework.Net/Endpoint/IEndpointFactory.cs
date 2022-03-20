using TnyFramework.Net.Command;
using TnyFramework.Net.Transport;
namespace TnyFramework.Net.Endpoint
{
    public interface IEndpointFactory<out TEndpoint, in TSetting>
        where TEndpoint : INetEndpoint
        where TSetting : IEndpointSetting
    {
        TEndpoint Create<TUserId>(TSetting setting, IEndpointContext context, ICertificateFactory<TUserId> certificateFactory);
    }
}
