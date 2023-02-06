// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using TnyFramework.Common.Event;
using TnyFramework.Net.Base;
using TnyFramework.Net.Command;
using TnyFramework.Net.Endpoint.Event;
using TnyFramework.Net.Transport;

namespace TnyFramework.Net.Endpoint
{

    public class EndpointKeeperManager : IEndpointKeeperManager
    {
        private const string DEFAULT_KEY = "default";

        private readonly ConcurrentDictionary<IMessagerType, IEndpointKeeper> endpointKeeperMap =
            new ConcurrentDictionary<IMessagerType, IEndpointKeeper>();

        private readonly ISessionKeeperSetting defaultSessionKeeperSetting;

        private readonly IDictionary<IMessagerType, ISessionKeeperSetting> sessionKeeperSettingMap;

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
                .ToDictionary(sessionKeeperSetting => (IMessagerType) MessagerType.ForGroup(sessionKeeperSetting.Name))
                .ToImmutableDictionary();
            sessionFactoryMap = sessionKeeperFactories.ToImmutableDictionary();
            createEventBus = CREATE_EVENT_BUS.ForkChild();
        }

        private IEndpointKeeper Create(IMessagerType messagerType, NetAccessMode accessMode)
        {
            if (!Equals(accessMode, NetAccessMode.Server))
                return null;
            if (!sessionKeeperSettingMap.TryGetValue(messagerType, out var setting))
            {
                setting = defaultSessionKeeperSetting;
            }
            if (!sessionFactoryMap.TryGetValue(setting.KeeperFactory, out var factory))
                throw new NullReferenceException($"{setting.KeeperFactory} factory is null");
            return factory.CreateKeeper(messagerType, setting);
        }

        public IEndpoint Online(ICertificate certificate, INetTunnel tunnel)
        {
            var keeper = LoadKeeper(certificate.MessagerType, tunnel.AccessMode);
            return keeper.Online(certificate, tunnel);
        }

        public IEndpointKeeper LoadKeeper(IMessagerType messagerType, NetAccessMode accessMode)
        {
            var keeper = FindKeeper(messagerType);
            if (keeper != null)
            {
                return keeper;
            }
            var newOne = Create(messagerType, accessMode);
            if (!endpointKeeperMap.TryAdd(messagerType, newOne))
                return endpointKeeperMap[messagerType];
            newOne.Start();
            createEventBus.Notify(newOne);
            return newOne;
        }

        public IEndpointKeeper FindKeeper(IMessagerType messagerType)
        {
            return endpointKeeperMap.TryGetValue(messagerType, out var keeper) ? keeper : default;
        }
    }

}
