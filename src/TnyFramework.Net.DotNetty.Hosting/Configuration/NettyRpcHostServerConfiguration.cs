// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using Microsoft.Extensions.DependencyInjection;
using TnyFramework.Net.DotNetty.Bootstrap;
using TnyFramework.Net.DotNetty.Hosting.Guide;
using TnyFramework.Net.Hosting.Rpc;

namespace TnyFramework.Net.DotNetty.Hosting.Configuration
{

    public class NettyRpcHostServerConfiguration
        : RpcEndPointHostServerConfiguration<INettyRpcHostServerConfiguration, INettyServerGuideUnitContext, INettyServerGuide, INettyServerSetting, INettyServerGuideSpec>,
        INettyRpcHostServerConfiguration
    {
        public static NettyRpcHostServerConfiguration CreateRpcServer(IServiceCollection unitContainer)
        {
            return new NettyRpcHostServerConfiguration(unitContainer);
        }

        private NettyRpcHostServerConfiguration(IServiceCollection unitContainer) : base(unitContainer)
        {

        }

        public override INettyRpcHostServerConfiguration RpcServer(INettyServerSetting setting, Action<INettyServerGuideSpec>? action = null)
        {
            Server(setting.Service, spec => {
                spec.Server(setting);
                action?.Invoke(spec);
            });
            OnAddRpcServer(setting.Service);
            return this;
        }

        public override INettyRpcHostServerConfiguration RpcServer(string name, int port, Action<INettyServerGuideSpec>? action = null)
        {
            Server(name, spec => {
                spec.Server(port);
                action?.Invoke(spec);
            });
            OnAddRpcServer(name);
            return this;
        }

        public override INettyRpcHostServerConfiguration RpcServer(string name, string host, int port, Action<INettyServerGuideSpec>? action = null)
        {
            Server(name, spec => {
                spec.Server(host, port);
                action?.Invoke(spec);
            });
            OnAddRpcServer(name);
            return this;
        }

        public INettyRpcHostServerConfiguration RpcServer(string name, string host, int port, bool libuv,
            Action<INettyServerGuideSpec>? action = null)
        {
            Server(name, spec => {
                spec.Server(host, port, libuv);
                action?.Invoke(spec);
            });
            OnAddRpcServer(name);
            return this;
        }

        public override INettyRpcHostServerConfiguration RpcServer(string name, string serveName, string host, int port,
            Action<INettyServerGuideSpec>? action = null)
        {
            Server(name, spec => {
                spec.Server(serveName, host, port);
                action?.Invoke(spec);
            });
            OnAddRpcServer(name);
            return this;
        }

        public INettyRpcHostServerConfiguration RpcServer(string name, string serveName, string host, int port, bool libuv,
            Action<INettyServerGuideSpec>? action = null)
        {
            Server(name, spec => {
                spec.Server(serveName, host, port, libuv);
                action?.Invoke(spec);
            });
            OnAddRpcServer(name);
            return this;
        }

        protected override void OnAddRpcServer(string name)
        {
            // EndpointConfigure(sessionSpec => sessionSpec
            //     .CustomSessionConfigure(UnitContainer.UnitName<ISessionKeeperSettingSpec>(name), settingSpec => settingSpec
            //         .UserType(name)
            //         .KeeperFactory(RPC_SESSION_KEEPER_NAME)));
        }

        protected override INettyServerGuideSpec CreateServerGuideSpec(string name)
        {
            return new NettyServerGuideSpec(name, NetUnitContext, UnitContainer);
        }
    }

}
