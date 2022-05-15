using TnyFramework.DI.Container;

namespace TnyFramework.Net.Endpoint
{

    public class SessionKeeperSetting : ISessionKeeperSetting
    {
        private readonly SessionSetting sessionSetting = new SessionSetting();

        public string Name { get; set; }

        public string KeeperFactory { get; set; } = Unit.DefaultName<SessionKeeperFactory<object>>();

        public string SessionFactory { get; set; } = Unit.DefaultName<SessionFactory>();

        public long OfflineCloseDelay { get; set; }

        public int OfflineMaxSize { get; set; }

        public long ClearSessionInterval { get; set; } = 60000;

        public ISessionSetting SessionSetting => sessionSetting;

        public SessionSetting GetSessionSetting()
        {
            return sessionSetting;
        }
    }

}
