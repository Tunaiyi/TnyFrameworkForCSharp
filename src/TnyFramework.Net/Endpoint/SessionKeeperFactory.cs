// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
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
            return CreateKeeper(messagerType, (ISessionKeeperSetting) setting);
        }

        public ISessionKeeper CreateKeeper(IMessagerType messagerType, ISessionKeeperSetting setting)
        {
            return factories.TryGetValue(setting.SessionFactory, out var factory)
                ? new SessionKeeper<TUserId>(messagerType, factory, setting)
                : throw new NullReferenceException();
        }
    }

}
