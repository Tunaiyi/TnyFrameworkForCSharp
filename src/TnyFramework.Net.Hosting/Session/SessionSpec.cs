// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using TnyFramework.DI.Units;
using TnyFramework.Net.Command.Auth;
using TnyFramework.Net.Session;

namespace TnyFramework.Net.Hosting.Session;

public class SessionSpec : UnitSpec<ISessionKeeperManager, ISessionUnitContext>, ISessionSpec, ISessionUnitContext
{
    private readonly UnitCollectionSpec<ISessionFactory, ISessionUnitContext> sessionFactorySpec;
    private readonly SessionKeeperSettingSpec defaultSessionKeeperSetting;
    private readonly SessionKeeperSettingSpecs customSessionKeeperSettingSpecs;
    private readonly UnitCollectionSpec<ISessionKeeperFactory, ISessionUnitContext> sessionKeeperFactorySpec;
    private readonly UnitSpec<IContactAuthenticator, ISessionUnitContext> contactAuthenticatorSpec;

    private IServiceCollection UnitContainer { get; }

    public SessionSpec(IServiceCollection container)
    {
        UnitContainer = container;
        sessionFactorySpec = UnitCollectionSpec.Units<ISessionFactory, ISessionUnitContext>()
            .AddDefault<SessionFactory>();
        sessionKeeperFactorySpec = UnitCollectionSpec.Units<ISessionKeeperFactory, ISessionUnitContext>();
        // 默认  SessionKeeper 配置
        defaultSessionKeeperSetting = SessionKeeperSettingSpec.New(spec => spec
            .UnitName("DefaultSession").Default(DefaultSessionKeeperSetting));
        // 自定义 SessionKeeper 配置
        customSessionKeeperSettingSpecs = new SessionKeeperSettingSpecs();
        contactAuthenticatorSpec = Unit<IContactAuthenticator, ISessionUnitContext>()
            .UnitName("DefaultContactAuthenticator").Default(DefaultContactAuthenticator);
        Default(DefaultEndpointKeeperManager);
    }

    public ISessionSpec DefaultSessionKeeperFactory(UnitCreator<ISessionKeeperFactory, ISessionUnitContext> defaultSessionFactory)
    {
        sessionKeeperFactorySpec.AddDefault(defaultSessionFactory);
        return this;
    }

    public ISessionSpec SessionFactoryConfigure(Action<UnitCollectionSpec<ISessionFactory, ISessionUnitContext>> action)
    {
        action.Invoke(sessionFactorySpec);
        return this;
    }

    public ISessionSpec SessionKeeperFactory()
    {
        sessionKeeperFactorySpec.Add(_ => new SessionKeeperFactory());
        return this;
    }

    public ISessionSpec SessionKeeperFactory(string name)
    {
        sessionKeeperFactorySpec.Add(name, _ => new SessionKeeperFactory());
        return this;
    }

    public ISessionSpec DefaultSessionConfigure(Action<ISessionKeeperSettingSpec> action)
    {
        action.Invoke(defaultSessionKeeperSetting);
        return this;
    }

    public ISessionSpec CustomSessionConfigure(Action<ISessionKeeperSettingSpec> action)
    {
        customSessionKeeperSettingSpecs.AddSpec(action);
        return this;
    }

    public ISessionSpec CustomSessionConfigure(string name, Action<ISessionKeeperSettingSpec> action)
    {
        customSessionKeeperSettingSpecs.AddSpec(name, action);
        return this;
    }

    public ISessionSpec SessionKeeperFactoryConfigure(Action<IUnitCollectionSpec<ISessionKeeperFactory, ISessionUnitContext>> action)
    {
        action.Invoke(sessionKeeperFactorySpec);
        return this;
    }

    public IContactAuthenticator LoadContactAuthenticator()
    {
        return contactAuthenticatorSpec.Load(this, UnitContainer);
    }

    public ISessionKeeperSetting LoadDefaultSessionKeeperSetting()
    {
        return defaultSessionKeeperSetting.Load(this, UnitContainer);
    }

    public IList<ISessionKeeperSetting> LoadCustomSessionKeeperSettings()
    {
        return customSessionKeeperSettingSpecs.Load(this, UnitContainer);
    }

    public IDictionary<string, ISessionKeeperFactory> LoadSessionKeeperFactories()
    {
        return sessionKeeperFactorySpec.LoadDictionary(this, UnitContainer);
    }

    public IDictionary<string, ISessionFactory> LoadSessionFactories()
    {
        return sessionFactorySpec.LoadDictionary(this, UnitContainer);
    }

    public ISessionKeeperManager LoadSessionKeeperManager()
    {
        return Load(this, UnitContainer);
    }

    public static ISessionKeeperSetting DefaultSessionKeeperSetting(ISessionUnitContext context)
    {
        return new SessionKeeperSetting {
            Name = "default"
        };
    }

    public static IContactAuthenticator DefaultContactAuthenticator(ISessionUnitContext context)
    {
        return new ContactAuthenticateService(context.LoadSessionKeeperManager());
    }

    public static ISessionKeeperManager DefaultEndpointKeeperManager(ISessionUnitContext context)
    {
        return new SessionKeeperManager(
            context.LoadDefaultSessionKeeperSetting(),
            context.LoadCustomSessionKeeperSettings(),
            context.LoadSessionKeeperFactories());
    }
}
