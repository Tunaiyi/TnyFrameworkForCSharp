// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using Microsoft.Extensions.DependencyInjection;
using TnyFramework.DI.Units;
using TnyFramework.Net.Base;
using TnyFramework.Net.DotNetty.Configuration.Guide;
using TnyFramework.Net.DotNetty.Configuration.Rpc;
using TnyFramework.Net.Rpc;
using TnyFramework.Net.Rpc.Auth;
using TnyFramework.Net.Transport;

namespace TnyFramework.Net.DotNetty.Configuration
{

    public class RpcServerConfiguration : NettyServerConfiguration, IRpcServerConfiguration
    {
        private const string RPC_SESSION_KEEPER_NAME = "RpcSessionKeeper";

        private readonly RpcUnitContext rpcUnitContext;

        private UnitSpec<IIdGenerator, IRpcUnitContext> IdGeneratorSpec { get; }

        public static RpcServerConfiguration CreateRpcServer(IServiceCollection unitContainer)
        {
            return new RpcServerConfiguration(unitContainer);
        }

        private RpcServerConfiguration(IServiceCollection unitContainer) : base(unitContainer)
        {
            rpcUnitContext = new RpcUnitContext(NetUnitContext, unitContainer);
            AddController<RpcAuthController>()
                .AddAuthenticateValidators(spec => spec.Creator(DefaultRpcPasswordValidator))
                .AddAuthenticateValidators(spec => spec.Creator(DefaultRpcTokenValidator))
                .EndpointConfigure(spec => spec
                    .SessionKeeperFactory<RpcAccessIdentify>(RPC_SESSION_KEEPER_NAME)
                    .DefaultSessionConfigure(keeper => keeper.KeeperFactory(RPC_SESSION_KEEPER_NAME))
                );
            IdGeneratorSpec = UnitSpec.Unit<IIdGenerator, IRpcUnitContext>()
                .Default<AutoIncrementIdGenerator>();
        }

        public RpcServerConfiguration RpcServer(ServerSetting setting, Action<INetServerGuideSpec<RpcAccessIdentify>> action = null)
        {
            Server<RpcAccessIdentify>(setting.Name, spec => {
                spec.Server(setting);
                action?.Invoke(spec);
            });
            OnAddRpcServer(setting.Name);
            return this;
        }

        public RpcServerConfiguration RpcServer(string name, int port, Action<INetServerGuideSpec<RpcAccessIdentify>> action = null)
        {
            Server<RpcAccessIdentify>(name, spec => {
                spec.Server(port);
                action?.Invoke(spec);
            });
            OnAddRpcServer(name);
            return this;
        }

        public RpcServerConfiguration RpcServer(string name, string host, int port, Action<INetServerGuideSpec<RpcAccessIdentify>> action = null)
        {
            Server<RpcAccessIdentify>(name, spec => {
                spec.Server(host, port);
                action?.Invoke(spec);
            });
            OnAddRpcServer(name);
            return this;
        }

        public RpcServerConfiguration RpcServer(string name, string host, int port, bool libuv,
            Action<INetServerGuideSpec<RpcAccessIdentify>> action = null)
        {
            Server<RpcAccessIdentify>(name, spec => {
                spec.Server(host, port, libuv);
                action?.Invoke(spec);
            });
            OnAddRpcServer(name);
            return this;
        }

        public RpcServerConfiguration RpcServer(string name, string serveName, string host, int port,
            Action<INetServerGuideSpec<RpcAccessIdentify>> action = null)
        {
            Server<RpcAccessIdentify>(name, spec => {
                spec.Server(serveName, host, port);
                action?.Invoke(spec);
            });
            OnAddRpcServer(name);
            return this;
        }

        public RpcServerConfiguration RpcServer(string name, string serveName, string host, int port, bool libuv,
            Action<INetServerGuideSpec<RpcAccessIdentify>> action = null)
        {
            Server<RpcAccessIdentify>(name, spec => {
                spec.Server(serveName, host, port, libuv);
                action?.Invoke(spec);
            });
            OnAddRpcServer(name);
            return this;
        }

        private void OnAddRpcServer(string name)
        {
            // EndpointConfigure(endpointSpec => endpointSpec
            //     .CustomSessionConfigure(UnitContainer.UnitName<ISessionKeeperSettingSpec>(name), settingSpec => settingSpec
            //         .UserType(name)
            //         .KeeperFactory(RPC_SESSION_KEEPER_NAME)));
        }

        public RpcServerConfiguration RpcAuthServiceSpecConfigure(Action<RpcAuthServiceSpec> action)
        {
            action.Invoke(rpcUnitContext.RpcAuthServiceSpec);
            return this;
        }

        public RpcServerConfiguration IdGeneratorConfigure(Action<UnitSpec<IIdGenerator, IRpcUnitContext>> action)
        {
            action.Invoke(IdGeneratorSpec);
            return this;
        }

        private IAuthenticateValidator DefaultRpcPasswordValidator(INetUnitContext context)
        {
            return new RpcPasswordValidator(IdGeneratorSpec.Load(rpcUnitContext, UnitContainer), rpcUnitContext.LoadRpcAuthService());
        }

        private IAuthenticateValidator DefaultRpcTokenValidator(INetUnitContext context)
        {
            return new RpcTokenValidator(IdGeneratorSpec.Load(rpcUnitContext, UnitContainer), rpcUnitContext.LoadRpcAuthService());
        }
    }

}
