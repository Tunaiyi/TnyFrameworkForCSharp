using System.Collections.Generic;
using System.Collections.Immutable;
namespace TnyFramework.Net.Endpoint
{
    public class SessionKeeperFactory<TUserId> : ISessionKeeperFactory
    {
        private readonly IDictionary<string, ISessionFactory> factories;


        public SessionKeeperFactory(IDictionary<string, ISessionFactory> sessionFactories)
        {
            factories = sessionFactories.ToImmutableDictionary();
        }


        public IEndpointKeeper CreateKeeper(string userType, IEndpointKeeperSetting setting)
        {
            return CreateKeeper(userType, (ISessionKeeperSetting)setting);
        }


        public ISessionKeeper CreateKeeper(string userType, ISessionKeeperSetting setting)
        {
            return factories.TryGetValue(setting.SessionFactory, out var factory) ? new SessionKeeper<TUserId>(userType, factory, setting) : null;
        }
    }
}
