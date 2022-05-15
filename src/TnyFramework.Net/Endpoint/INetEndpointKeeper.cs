namespace TnyFramework.Net.Endpoint
{

    public interface INetEndpointKeeper : IEndpointKeeper
    {
        void NotifyEndpointOnline(IEndpoint endpoint);

        void NotifyEndpointOffline(IEndpoint endpoint);

        void NotifyEndpointClose(IEndpoint endpoint);
    }

}
