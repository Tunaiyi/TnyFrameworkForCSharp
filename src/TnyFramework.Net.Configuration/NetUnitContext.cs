// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using TnyFramework.DI.Units;
using TnyFramework.Net.Base;
using TnyFramework.Net.Command.Auth;
using TnyFramework.Net.Command.Dispatcher;
using TnyFramework.Net.Command.Tasks;
using TnyFramework.Net.Configuration.App;
using TnyFramework.Net.Configuration.Endpoint;
using TnyFramework.Net.Plugin;
using TnyFramework.Net.Rpc;

namespace TnyFramework.Net.Configuration
{

    public class NetUnitContext : INetUnitContext
    {
        private IServiceCollection UnitContainer { get; }

        public NetAppContextSpec AppContextSpec { get; }

        public EndpointSpec EndpointSpec { get; }

        public IEndpointUnitContext EndpointUnitContext => EndpointSpec;

        public UnitSpec<MessageDispatcherContext, INetUnitContext> MessageDispatcherContextSpec { get; }

        public UnitSpec<IMessageDispatcher, INetUnitContext> MessageDispatcherSpec { get; }

        public UnitSpec<IRpcInvokeNodeManager, INetUnitContext> RpcRemoteServiceManagerSpec { get; }

        public UnitSpec<ICommandBoxFactory, INetUnitContext> CommandBoxFactorySpec { get; }

        public UnitCollectionSpec<ICommandPlugin, INetUnitContext> CommandPluginSpecs { get; }

        public UnitCollectionSpec<IAuthenticationValidator, INetUnitContext> AuthenticateValidatorSpecs { get; }

        public NetUnitContext(IServiceCollection unitContainer)
        {
            UnitContainer = unitContainer;

            RpcRemoteServiceManagerSpec = UnitSpec.Unit<IRpcInvokeNodeManager, INetUnitContext>()
                .Default(DefaultRpcRemoteServiceManager);

            // MessageDispatcher 
            MessageDispatcherSpec = UnitSpec.Unit<IMessageDispatcher, INetUnitContext>()
                .Default(DefaultMessageDispatcher);

            MessageDispatcherContextSpec = UnitSpec.Unit<MessageDispatcherContext, INetUnitContext>()
                .Default(DefaultMessageDispatcherContext);

            // CommandTaskBoxProcessor
            CommandBoxFactorySpec = UnitSpec.Unit<ICommandBoxFactory, INetUnitContext>()
                .Default<DefaultCommandBoxFactory>();

            // CommandPlugin
            CommandPluginSpecs = UnitCollectionSpec.Units<ICommandPlugin, INetUnitContext>();

            // AuthenticateValidator
            AuthenticateValidatorSpecs = UnitCollectionSpec.Units<IAuthenticationValidator, INetUnitContext>();

            // Endpoint
            EndpointSpec = new EndpointSpec(UnitContainer);

            // AppContext
            AppContextSpec = new NetAppContextSpec();

        }

        public void Load()
        {
            LoadAppContext();
            LoadAuthenticateValidators();
            LoadCommandPlugins();
            LoadMessageDispatcher();
            LoadCommandBoxFactory();
            LoadMessageDispatcherContext();
            LoadRpcRemoteServiceManager();
        }

        public MessageDispatcherContext LoadMessageDispatcherContext()
        {
            return MessageDispatcherContextSpec.Load(this, UnitContainer);
        }

        public IRpcInvokeNodeManager LoadRpcRemoteServiceManager()
        {
            return RpcRemoteServiceManagerSpec.Load(this, UnitContainer);
        }

        public IMessageDispatcher LoadMessageDispatcher()
        {
            return MessageDispatcherSpec.Load(this, UnitContainer);
        }

        public ICommandBoxFactory LoadCommandBoxFactory()
        {
            return CommandBoxFactorySpec.Load(this, UnitContainer);
        }

        public INetAppContext LoadAppContext()
        {
            return AppContextSpec.Load(this, UnitContainer);
        }

        public IList<ICommandPlugin> LoadCommandPlugins()
        {
            return CommandPluginSpecs.Load(this, UnitContainer);
        }

        public IList<IAuthenticationValidator> LoadAuthenticateValidators()
        {
            return AuthenticateValidatorSpecs.Load(this, UnitContainer);
        }

        private static MessageDispatcherContext DefaultMessageDispatcherContext(INetUnitContext context)
        {
            return new MessageDispatcherContext(
                context.LoadAppContext(),
                context.LoadCommandPlugins(),
                context.LoadAuthenticateValidators());
        }

        private static MessageDispatcher DefaultMessageDispatcher(INetUnitContext context)
        {
            return new MessageDispatcher(context.LoadMessageDispatcherContext(), context.EndpointUnitContext.LoadContactAuthenticator());
        }

        private static RpcServicerManager DefaultRpcRemoteServiceManager(INetUnitContext context)
        {
            return new RpcServicerManager();
        }
    }

}
