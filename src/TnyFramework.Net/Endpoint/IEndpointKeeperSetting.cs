namespace TnyFramework.Net.Endpoint
{
    public interface IEndpointKeeperSetting
    {
        string Name { get; }
        string KeeperFactory { get; }
    }
}
