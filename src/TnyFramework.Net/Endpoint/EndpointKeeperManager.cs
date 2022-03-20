using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using TnyFramework.Common.Event;
using TnyFramework.Net.Command;
using TnyFramework.Net.Endpoint.Event;
using TnyFramework.Net.Rpc;
using TnyFramework.Net.Transport;
namespace TnyFramework.Net.Endpoint
{
    public class EndpointKeeperManager : IEndpointKeeperManager
    {
        private const string DEFAULT_KEY = "default";

        private readonly ConcurrentDictionary<string, IEndpointKeeper> endpointKeeperMap = new ConcurrentDictionary<string, IEndpointKeeper>();

        private readonly ISessionKeeperSetting defaultSessionKeeperSetting;

        private readonly IDictionary<string, ISessionKeeperSetting> sessionKeeperSettingMap;

        private readonly IDictionary<string, ISessionKeeperFactory> sessionFactoryMap;

        private static readonly IEventBus<EndpointKeeperCreate> CREATE_EVENT_BUS = EventBuses.Create<EndpointKeeperCreate>();

        public static IEventBox<EndpointKeeperCreate> CreateEventBox => CREATE_EVENT_BUS;

        private readonly IEventBus<EndpointKeeperCreate> createEventBus;

        private IEventBox<EndpointKeeperCreate> CreateEvent => createEventBus;


        public EndpointKeeperManager(
            ISessionKeeperSetting defaultSessionKeeperSetting,
            IEnumerable<ISessionKeeperSetting> sessionKeeperSettings,
            IDictionary<string, ISessionKeeperFactory> sessionKeeperFactories)
        {
            this.defaultSessionKeeperSetting = defaultSessionKeeperSetting;
            sessionKeeperSettingMap = sessionKeeperSettings
                .ToDictionary(sessionKeeperSetting => sessionKeeperSetting.Name)
                .ToImmutableDictionary();
            sessionFactoryMap = sessionKeeperFactories.ToImmutableDictionary();
            createEventBus = CREATE_EVENT_BUS.ForkChild();
        }


        private IEndpointKeeper Create(string userType, TunnelMode tunnelMode)
        {
            if (!Equals(tunnelMode, TunnelMode.SERVER))
                return null;
            if (!sessionKeeperSettingMap.TryGetValue(userType, out var setting))
            {
                setting = defaultSessionKeeperSetting;
            }
            if (!sessionFactoryMap.TryGetValue(setting.KeeperFactory, out var factory))
                throw new NullReferenceException($"{setting.KeeperFactory} factory is null");
            return factory.CreateKeeper(userType, setting);
        }


        public IEndpoint Online(ICertificate certificate, INetTunnel tunnel)
        {
            var keeper = LoadKeeper<IEndpointKeeper>(certificate.UserType, tunnel.Mode);
            return keeper.Online(certificate, tunnel);
        }


        public TKeeper LoadKeeper<TKeeper>(string userType, TunnelMode tunnelMode) where TKeeper : IEndpointKeeper
        {
            var keeper = FindKeeper<TKeeper>(userType);
            if (keeper != null)
            {
                return keeper;
            }
            var newOne = Create(userType, tunnelMode);
            if (!endpointKeeperMap.TryAdd(userType, newOne))
                return (TKeeper)endpointKeeperMap[userType];
            newOne.Start();
            createEventBus.Notify(newOne);
            return (TKeeper)newOne;
        }


        public TKeeper FindKeeper<TKeeper>(string userType) where TKeeper : IEndpointKeeper
        {
            if (endpointKeeperMap.TryGetValue(userType, out var keeper))
            {
                return (TKeeper)keeper;
            }
            return default;
        }
    }
}
