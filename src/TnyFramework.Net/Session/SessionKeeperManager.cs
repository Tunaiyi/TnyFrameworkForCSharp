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
using TnyFramework.Common.EventBus;
using TnyFramework.Net.Application;
using TnyFramework.Net.Session.Event;
using TnyFramework.Net.Transport;

namespace TnyFramework.Net.Session;

public class SessionKeeperManager : ISessionKeeperManager
{
    // private const string DEFAULT_KEY = "default";

    private readonly ConcurrentDictionary<IContactType, ISessionKeeper?> sessionKeeperMap = new();

    private readonly ISessionKeeperSetting defaultSessionKeeperSetting;

    private readonly IDictionary<IContactType, ISessionKeeperSetting> sessionKeeperSettingMap;

    private readonly IDictionary<string, ISessionKeeperFactory> sessionFactoryMap;

    private static readonly IEventBus<SessionKeeperCreate> CREATE_EVENT_BUS = EventBuses.Create<SessionKeeperCreate>();

    public static IEventWatch<SessionKeeperCreate> CreateEventBox => CREATE_EVENT_BUS;

    private readonly IEventBus<SessionKeeperCreate> createEventBus;

    private IEventWatch<SessionKeeperCreate> CreateEvent => createEventBus;

    public SessionKeeperManager(
        ISessionKeeperSetting defaultSessionKeeperSetting,
        IEnumerable<ISessionKeeperSetting> sessionKeeperSettings,
        IDictionary<string, ISessionKeeperFactory> sessionKeeperFactories)
    {
        this.defaultSessionKeeperSetting = defaultSessionKeeperSetting;
        sessionKeeperSettingMap = sessionKeeperSettings
            .ToDictionary(sessionKeeperSetting => (IContactType) ContactType.ForGroup(sessionKeeperSetting.Name))
            .ToImmutableDictionary();
        sessionFactoryMap = sessionKeeperFactories.ToImmutableDictionary();
        createEventBus = CREATE_EVENT_BUS.ForkChild();
    }

    private ISessionKeeper Create(IContactType contactType)
    {
        if (!sessionKeeperSettingMap.TryGetValue(contactType, out var setting))
        {
            setting = defaultSessionKeeperSetting;
        }
        if (!sessionFactoryMap.TryGetValue(setting.KeeperFactory, out var factory))
            throw new NullReferenceException($"{setting.KeeperFactory} factory is null");
        return factory.CreateKeeper(contactType, setting);
    }

    public ISession? Online(ICertificate certificate, INetTunnel tunnel)
    {
        var keeper = LoadKeeper(certificate.ContactType, tunnel.AccessMode);
        return keeper?.Online(certificate, tunnel);
    }

    public ISessionKeeper? LoadKeeper(IContactType contactType, NetAccessMode accessMode)
    {
        var keeper = FindKeeper(contactType);
        if (keeper != null)
        {
            return keeper;
        }
        // if (!Equals(accessMode, NetAccessMode.Server))
        // {
        //     throw new ArgumentException();
        // }
        var newOne = Create(contactType);
        if (!sessionKeeperMap.TryAdd(contactType, newOne))
            return sessionKeeperMap[contactType];
        newOne.Start();
        createEventBus.Notify(newOne);
        return newOne;
    }

    public ISessionKeeper? FindKeeper(IContactType contactType)
    {
        return sessionKeeperMap.GetValueOrDefault(contactType);
    }
}
