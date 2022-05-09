using System.Collections.Generic;
using System.Collections.Immutable;
using TnyFramework.Net.Base;

namespace TnyFramework.Net.Endpoint
{
    public class SessionKeeperFactory<TUserId> : ISessionKeeperFactory
    {
        private readonly IDictionary<string, ISessionFactory> factories;


        public SessionKeeperFactory(IDictionary<string, ISessionFactory> sessionFactories)
        {
            factories = sessionFactories.ToImmutableDictionary();
        }


        public IEndpointKeeper CreateKeeper(IMessagerType messagerType, IEndpointKeeperSetting setting)
        {
            return CreateKeeper(messagerType, (ISessionKeeperSetting)setting);
        }


        public ISessionKeeper CreateKeeper(IMessagerType messagerType, ISessionKeeperSetting setting)
        {
            return factories.TryGetValue(setting.SessionFactory, out var factory) ? new SessionKeeper<TUserId>(messagerType, factory, setting) : null;
        }
    }
}
