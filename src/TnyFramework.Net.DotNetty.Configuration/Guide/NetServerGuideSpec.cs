// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using Microsoft.Extensions.DependencyInjection;
using TnyFramework.Net.Base;
using TnyFramework.Net.DotNetty.Bootstrap;

namespace TnyFramework.Net.DotNetty.Configuration.Guide
{

    public interface INetServerGuideBuilder
    {
        INettyServerGuide BuildGuide();
    }

    public class NetServerGuideSpec
        : NetGuideSpec<INettyServerGuide, INetServerGuideUnitContext, NetServerGuideUnitContext, INetServerGuideSpec>,
            INetServerGuideSpec, INetServerGuideBuilder
    {
        public INettyServerGuide? Guide { get; private set; }

        internal IServiceCollection UnitContainer { get; }

        public NetServerGuideSpec(string name, INetUnitContext unitContext, IServiceCollection unitContainer) :
            base(new NetServerGuideUnitContext(unitContext, unitContainer))
        {
            WithNamePrefix(name);
            UnitContainer = unitContainer;
            context.SetName(name);
            Default(c => new NettyServerGuide(
                c.LoadServerSetting(),
                c.LoadTunnelFactory(),
                c.LoadNetworkContext(),
                c.LoadChannelMaker()));
            context.ServerSettingSpec.ServiceName(name);
        }

        public INettyServerGuide BuildGuide()
        {
            if (Guide != null)
                return Guide;
            return Guide = Load(context, UnitContainer);
        }

        protected override INetServerGuideSpec Self()
        {
            return this;
        }

        public INetServerGuideSpec Server(IServerSetting setting)
        {
            var serverSettings = context.ServerSettingSpec;
            serverSettings.Unit(setting);
            return this;
        }

        public INetServerGuideSpec Server(int port)
        {
            var serverSettings = context.ServerSettingSpec;
            serverSettings.Port(port);
            return this;
        }

        public INetServerGuideSpec Server(string host, int port)
        {
            var serverSettings = context.ServerSettingSpec;
            serverSettings.Host(host).Port(port);
            return this;
        }

        public INetServerGuideSpec Server(string host, int port, bool libuv)
        {
            var serverSettings = context.ServerSettingSpec;
            serverSettings.Host(host).Port(port).Libuv(libuv);
            return this;

        }

        public INetServerGuideSpec Server(string serveName, string host, int port)
        {
            var serverSettings = context.ServerSettingSpec;
            serverSettings.ServeName(serveName).Host(host).Port(port);
            return this;
        }

        public INetServerGuideSpec Server(string serveName, string host, int port, bool libuv)
        {

            var serverSettings = context.ServerSettingSpec;
            serverSettings.ServeName(serveName).Host(host).Port(port).Libuv(libuv);
            return this;
        }
    }

}
