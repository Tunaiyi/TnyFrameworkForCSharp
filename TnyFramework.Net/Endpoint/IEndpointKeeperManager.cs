using TnyFramework.Net.Command;
using TnyFramework.Net.Rpc;
using TnyFramework.Net.Transport;
namespace TnyFramework.Net.Endpoint
{
    /// <summary>
    /// Endpoint 管理器
    /// </summary>
    public interface IEndpointKeeperManager
    {
        IEndpoint Online(ICertificate certificate, INetTunnel tunnel);

        TKeeper LoadKeeper<TKeeper>(string userType, TunnelMode tunnelMode) where TKeeper : IEndpointKeeper;

        TKeeper FindKeeper<TKeeper>(string userType) where TKeeper : IEndpointKeeper;
    }
}
