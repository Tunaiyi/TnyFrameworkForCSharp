using TnyFramework.Net.Base;

namespace TnyFramework.Net.Endpoint
{

    public interface IEndpointKeeperFactory
    {
        IEndpointKeeper CreateKeeper(IMessagerType messagerType, IEndpointKeeperSetting setting);
    }

    public interface IEndpointKeeperFactory<out TEndpointKeeper, in TSetting> : IEndpointKeeperFactory
        where TEndpointKeeper : IEndpointKeeper
        where TSetting : IEndpointKeeperSetting
    {
        TEndpointKeeper CreateKeeper(IMessagerType messagerType, TSetting setting);
    }

}
