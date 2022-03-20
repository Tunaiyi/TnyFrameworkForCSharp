namespace TnyFramework.Net.Endpoint
{
    public interface IEndpointKeeperFactory
    {
        IEndpointKeeper CreateKeeper(string userType, IEndpointKeeperSetting setting);
    }

    public interface IEndpointKeeperFactory<out TEndpointKeeper, in TSetting> : IEndpointKeeperFactory
        where TEndpointKeeper : IEndpointKeeper
        where TSetting : IEndpointKeeperSetting
    {
        TEndpointKeeper CreateKeeper(string userType, TSetting setting);
    }
}
