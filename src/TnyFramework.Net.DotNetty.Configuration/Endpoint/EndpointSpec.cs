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
using TnyFramework.Net.Endpoint;

namespace TnyFramework.Net.DotNetty.Configuration.Endpoint
{

    public class EndpointSpec : UnitSpec<IEndpointKeeperManager, IEndpointUnitContext>, IEndpointSpec, IEndpointUnitContext
    {
        private readonly UnitCollectionSpec<ISessionFactory, IEndpointUnitContext> sessionFactorySpec;
        private readonly SessionKeeperSettingSpec defaultSessionKeeperSetting;
        private readonly SessionKeeperSettingSpecs customSessionKeeperSettingSpecs;
        private readonly UnitCollectionSpec<ISessionKeeperFactory, IEndpointUnitContext> sessionKeeperFactorySpec;
        private readonly UnitSpec<IMessagerAuthenticator, IEndpointUnitContext> messagerAuthenticatorSpec;

        private IServiceCollection UnitContainer { get; }

        public EndpointSpec(IServiceCollection container)
        {
            UnitContainer = container;
            sessionFactorySpec = UnitCollectionSpec.Units<ISessionFactory, IEndpointUnitContext>()
                .AddDefault<SessionFactory>();
            sessionKeeperFactorySpec = UnitCollectionSpec.Units<ISessionKeeperFactory, IEndpointUnitContext>();
            // 默认  SessionKeeper 配置
            defaultSessionKeeperSetting = SessionKeeperSettingSpec.New(spec => spec
                .UnitName("DefaultSession").Default(DefaultSessionKeeperSetting));
            // 自定义 SessionKeeper 配置
            customSessionKeeperSettingSpecs = new SessionKeeperSettingSpecs();
            messagerAuthenticatorSpec = Unit<IMessagerAuthenticator, IEndpointUnitContext>()
                .UnitName("DefaultMessagerAuthenticator").Default(DefaultMessagerAuthenticator);
            Default(DefaultEndpointKeeperManager);
        }

        public IEndpointSpec DefaultSessionKeeperFactory(UnitCreator<ISessionKeeperFactory, IEndpointUnitContext> defaultSessionFactory)
        {
            sessionKeeperFactorySpec.AddDefault(defaultSessionFactory);
            return this;
        }

        public IEndpointSpec SessionFactoryConfigure(Action<UnitCollectionSpec<ISessionFactory, IEndpointUnitContext>> action)
        {
            action.Invoke(sessionFactorySpec);
            return this;
        }

        public IEndpointSpec SessionKeeperFactory<TUserId>()
        {
            sessionKeeperFactorySpec.Add(context => new SessionKeeperFactory<TUserId>(context.LoadSessionFactories()));
            return this;
        }

        public IEndpointSpec SessionKeeperFactory<TUserId>(string name)
        {
            sessionKeeperFactorySpec.Add(name, context => new SessionKeeperFactory<TUserId>(context.LoadSessionFactories()));
            return this;
        }

        public IEndpointSpec DefaultSessionConfigure(Action<ISessionKeeperSettingSpec> action)
        {
            action.Invoke(defaultSessionKeeperSetting);
            return this;
        }

        public IEndpointSpec CustomSessionConfigure(Action<ISessionKeeperSettingSpec> action)
        {
            customSessionKeeperSettingSpecs.AddSpec(action);
            return this;
        }

        public IEndpointSpec CustomSessionConfigure(string name, Action<ISessionKeeperSettingSpec> action)
        {
            customSessionKeeperSettingSpecs.AddSpec(name, action);
            return this;
        }

        public IEndpointSpec SessionKeeperFactoryConfigure(Action<IUnitCollectionSpec<ISessionKeeperFactory, IEndpointUnitContext>> action)
        {
            action.Invoke(sessionKeeperFactorySpec);
            return this;
        }

        public IMessagerAuthenticator LoadMessagerAuthenticator()
        {
            return messagerAuthenticatorSpec.Load(this, UnitContainer);
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

        public IEndpointKeeperManager LoadEndpointKeeperManager()
        {
            return Load(this, UnitContainer);
        }

        public static ISessionKeeperSetting DefaultSessionKeeperSetting(IEndpointUnitContext context)
        {
            return new SessionKeeperSetting {
                Name = "default"
            };
        }

        public static IMessagerAuthenticator DefaultMessagerAuthenticator(IEndpointUnitContext context)
        {
            return new MessagerAuthenticateService(context.LoadEndpointKeeperManager());
        }

        public static IEndpointKeeperManager DefaultEndpointKeeperManager(IEndpointUnitContext context)
        {
            return new EndpointKeeperManager(
                context.LoadDefaultSessionKeeperSetting(),
                context.LoadCustomSessionKeeperSettings(),
                context.LoadSessionKeeperFactories());
        }
    }

}
