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
using TnyFramework.Net.Application;
using TnyFramework.Net.Command.Auth;
using TnyFramework.Net.Hosting.Configuration;
using TnyFramework.Net.Hosting.Guide;
using TnyFramework.Net.Rpc.Auth;
using TnyFramework.Net.Transport;

namespace TnyFramework.Net.Hosting.Rpc
{

    public abstract class RpcEndPointHostServerConfiguration<TConfiguration, TContext, TGuide, TSetting, TSpec>
        : NetHostConfiguration<TConfiguration, TContext, TGuide, TSpec>,
            IRpcEndPointHostConfiguration<TGuide, TSetting, TContext, TConfiguration, TSpec>
        where TSetting : IServiceServerSetting
        where TGuide : INetServerGuide<TSetting>
        where TContext : INetGuideUnitContext
        where TConfiguration : IRpcEndPointHostConfiguration<TGuide, TSetting, TContext, TConfiguration, TSpec>
        where TSpec : INetGuideSpec<TGuide, TContext, TSpec>
    {
        private const string RPC_SESSION_KEEPER_NAME = "RpcSessionKeeper";

        private readonly RpcUnitContext rpcUnitContext;

        private UnitSpec<IIdGenerator, IRpcUnitContext> IdGeneratorSpec { get; }

        protected RpcEndPointHostServerConfiguration(IServiceCollection unitContainer) : base(unitContainer)
        {
            rpcUnitContext = new RpcUnitContext(NetUnitContext, unitContainer);
            AddController<RpcAuthController>()
                .AddAuthenticateValidators(spec => spec.Creator(DefaultRpcPasswordValidator))
                .AddAuthenticateValidators(spec => spec.Creator(DefaultRpcTokenValidator))
                .EndpointConfigure(spec => spec.SessionKeeperFactory(RPC_SESSION_KEEPER_NAME)
                    .DefaultSessionConfigure(keeper => keeper.KeeperFactory(RPC_SESSION_KEEPER_NAME))
                );
            IdGeneratorSpec = UnitSpec.Unit<IIdGenerator, IRpcUnitContext>()
                .Default<AutoIncrementIdGenerator>();
        }

        public abstract TConfiguration RpcServer(TSetting setting, Action<TSpec>? action = null);

        public abstract TConfiguration RpcServer(string name, int port, Action<TSpec>? action = null);

        public abstract TConfiguration RpcServer(string name, string host, int port, Action<TSpec>? action = null);

        public abstract TConfiguration RpcServer(string name, string serveName, string host, int port, Action<TSpec>? action = null);

        public TConfiguration RpcAuthServiceSpecConfigure(Action<RpcAuthServiceSpec> action)
        {
            action.Invoke(rpcUnitContext.RpcAuthServiceSpec);
            return Self;
        }

        public TConfiguration IdGeneratorConfigure(Action<UnitSpec<IIdGenerator, IRpcUnitContext>> action)
        {
            action.Invoke(IdGeneratorSpec);
            return Self;
        }

        private IAuthenticationValidator DefaultRpcPasswordValidator(INetUnitContext context)
        {
            return new RpcPasswordValidator(IdGeneratorSpec.Load(rpcUnitContext, UnitContainer), rpcUnitContext.LoadRpcAuthService());
        }

        private IAuthenticationValidator DefaultRpcTokenValidator(INetUnitContext context)
        {
            return new RpcTokenValidator(IdGeneratorSpec.Load(rpcUnitContext, UnitContainer), rpcUnitContext.LoadRpcAuthService());
        }

        protected abstract void OnAddRpcServer(string name);
    }

}