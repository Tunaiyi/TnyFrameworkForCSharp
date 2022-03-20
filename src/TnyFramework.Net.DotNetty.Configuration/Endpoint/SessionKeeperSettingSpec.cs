using System;
using TnyFramework.DI.Units;
using TnyFramework.Net.Endpoint;
namespace TnyFramework.Net.DotNetty.Configuration.Endpoint
{
    public interface ISessionKeeperSettingSpec : IUnitSpec<ISessionKeeperSetting, IEndpointUnitContext>
    {
        SessionKeeperSettingSpec UserType(string value);

        SessionKeeperSettingSpec KeeperFactory(string value);

        SessionKeeperSettingSpec SessionFactory(string value);

        SessionKeeperSettingSpec OfflineCloseDelay(long value);

        SessionKeeperSettingSpec OfflineMaxSize(int value);

        SessionKeeperSettingSpec ClearSessionInterval(long value);
    }

    public class SessionKeeperSettingSpec : UnitSpec<ISessionKeeperSetting, IEndpointUnitContext>, ISessionKeeperSettingSpec
    {
        private readonly SessionKeeperSetting setting = new SessionKeeperSetting();


        public static SessionKeeperSettingSpec New()
        {
            return new SessionKeeperSettingSpec();
        }


        public static SessionKeeperSettingSpec New(Action<SessionKeeperSettingSpec> init)
        {
            return new SessionKeeperSettingSpec(init);
        }


        public SessionKeeperSettingSpec(Action<SessionKeeperSettingSpec> init = null) : base()
        {
            Default(_ => setting);
            init?.Invoke(this);
        }


        public SessionKeeperSettingSpec UserType(string value)
        {
            setting.Name = value;
            return this;
        }


        public SessionKeeperSettingSpec KeeperFactory(string value)
        {
            setting.KeeperFactory = value;
            return this;
        }


        public SessionKeeperSettingSpec SessionFactory(string value)
        {
            setting.SessionFactory = value;
            return this;
        }


        public SessionKeeperSettingSpec OfflineCloseDelay(long value)
        {
            setting.OfflineCloseDelay = value;
            return this;
        }


        public SessionKeeperSettingSpec OfflineMaxSize(int value)
        {
            setting.OfflineMaxSize = value;
            return this;
        }


        public SessionKeeperSettingSpec ClearSessionInterval(long value)
        {
            setting.ClearSessionInterval = value;
            return this;
        }
    }
}
