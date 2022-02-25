namespace TnyFramework.Net.Endpoint
{
    public interface ISessionKeeperSetting : IEndpointKeeperSetting
    {
        ISessionSetting SessionSetting { get; }

        string SessionFactory { get; }

        long OfflineCloseDelay { get; }

        int OfflineMaxSize { get; }

        long ClearSessionInterval { get; }
    }
}
