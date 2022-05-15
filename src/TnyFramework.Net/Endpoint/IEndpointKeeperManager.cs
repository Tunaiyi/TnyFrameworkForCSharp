using TnyFramework.Net.Base;
using TnyFramework.Net.Command;
using TnyFramework.Net.Transport;

namespace TnyFramework.Net.Endpoint
{

    /// <summary>
    /// Endpoint 管理器
    /// </summary>
    public interface IEndpointKeeperManager
    {
        IEndpoint Online(ICertificate certificate, INetTunnel tunnel);

        TKeeper LoadKeeper<TKeeper>(IMessagerType messagerType, TunnelMode tunnelMode) where TKeeper : IEndpointKeeper;

        TKeeper FindKeeper<TKeeper>(IMessagerType messagerType) where TKeeper : IEndpointKeeper;
    }

}
