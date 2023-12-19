// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using TnyFramework.DI.Units;
using TnyFramework.Net.Endpoint;

namespace TnyFramework.Net.Configuration.Endpoint
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

        public SessionKeeperSettingSpec(Action<SessionKeeperSettingSpec>? init = null)
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
