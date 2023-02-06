// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TnyFramework.Common.Logger;
using TnyFramework.DI.Container;
using TnyFramework.DI.Extensions;
using TnyFramework.DI.Units;
using TnyFramework.Net.Attributes;
using TnyFramework.Net.Command.Auth;
using TnyFramework.Net.Command.Dispatcher;
using TnyFramework.Net.Command.Processor;
using TnyFramework.Net.DotNetty.Bootstrap;
using TnyFramework.Net.DotNetty.Configuration.Endpoint;
using TnyFramework.Net.DotNetty.Configuration.Guide;
using TnyFramework.Net.Plugin;
using TnyFramework.Net.Rpc;
using TnyFramework.Net.Rpc.Auth;

namespace TnyFramework.Net.DotNetty.Configuration
{

    public class NettyServerConfiguration : INettyServerConfiguration
    {
        private static ILogger _LOGGER;

        private static ILogger Logger => _LOGGER ??= LogFactory.Logger<NettyServerConfiguration>();

        // private readonly UnitSpec<NettyServerGuide, INetUnitContext> serverGuideSpec;

        // private readonly UnitCollectionSpec<NettyServerGuide, INetServerGuideUnitContext> serverGuideSpecs;

        private readonly IList<INetServerGuideBuilder> netServerGuideBuilders = new List<INetServerGuideBuilder>();

        private readonly IList<ICustomServerConfiguration> configurators = new List<ICustomServerConfiguration>();

        private readonly List<INettyServerGuide> guides = new List<INettyServerGuide>();

        private bool initialized;

        protected NetUnitContext NetUnitContext { get; }

        protected IServiceCollection UnitContainer { get; }

        public static NettyServerConfiguration CreateNetServer(IServiceCollection unitContainer)
        {
            return new NettyServerConfiguration(unitContainer);
        }

        protected NettyServerConfiguration(IServiceCollection unitContainer)
        {
            UnitContainer = unitContainer;
            NetUnitContext = new NetUnitContext(unitContainer);
            // serverGuideSpecs = UnitCollectionSpec.Units<INetServerGuideUnitSpec, INetServerGuideUnitContext>();
            // serverGuideSpecs = UnitSpec.Unit<NettyServerGuide, INetUnitContext>()
            //     .Default(c => new NettyServerGuide(
            //         c.LoadServerSetting(),
            //         c.LoadTunnelFactory(),
            //         c.LoadNetworkContext(),
            //         c.LoadChannelMaker()));
        }

        public NettyServerConfiguration Server<TUserId>(string name, Action<INetServerGuideSpec<TUserId>> action)
        {
            var spec = new NetServerGuideSpec<TUserId>(name, NetUnitContext, UnitContainer);
            action(spec);
            netServerGuideBuilders.Add(spec);
            return this;
        }

        public NettyServerConfiguration AppContext(int serverId, string name)
        {
            NetUnitContext.AppContextSpec
                .ServerId(serverId)
                .AppName(name);
            return this;
        }

        public NettyServerConfiguration AppContext(int serverId, string name, string appType, string scope)
        {
            NetUnitContext.AppContextSpec
                .ServerId(serverId)
                .AppName(name)
                .AppType(appType)
                .ScopeType(scope);
            return this;
        }

        public NettyServerConfiguration AppContextConfigure(Action<INetAppContextSpec> action)
        {
            action.Invoke(NetUnitContext.AppContextSpec);
            return this;
        }

        public NettyServerConfiguration MessageDispatcherConfigure(
            Action<IUnitSpec<IMessageDispatcher, INetUnitContext>> action)
        {
            action.Invoke(NetUnitContext.MessageDispatcherSpec);
            return this;
        }

        public NettyServerConfiguration CommandTaskBoxProcessor(
            Action<IUnitSpec<ICommandTaskBoxProcessor, INetUnitContext>> action)
        {
            action.Invoke(NetUnitContext.CommandTaskBoxProcessorSpec);
            return this;
        }

        public NettyServerConfiguration AddController<TController>() where TController : class, IController
        {
            UnitContainer.AddSingletonUnit<TController>();
            Logger.LogInformation("AddController : {Controller}", typeof(TController));
            return this;
        }

        public NettyServerConfiguration AddController(IController controller)
        {
            UnitContainer.AddSingletonUnit(controller);
            Logger.LogInformation("AddController : {Controller}", controller.GetType());
            return this;
        }

        public NettyServerConfiguration AddController(Func<IServiceProvider, IController> factory)
        {
            UnitContainer.AddSingletonUnit(factory);
            return this;
        }

        public NettyServerConfiguration AddController<TController>(Func<IServiceProvider, TController> factory)
            where TController : IController
        {
            UnitContainer.AddSingletonUnit(factory);
            Logger.LogInformation("AddController : {Controller}", typeof(TController));
            return this;
        }

        public NettyServerConfiguration AddControllers()
        {
            return AddControllers(RpcTypeSelector.Controllers);
        }

        public NettyServerConfiguration AddControllers(ICollection<Assembly> assemblies)
        {
            foreach (var assembly in assemblies)
            {
                AddControllers(assembly.GetTypes());
            }
            return this;
        }

        private NettyServerConfiguration AddControllers(IEnumerable<Type> types)
        {

            foreach (var type in types)
            {
                var iControllerType = typeof(IController);
                if (type.IsAbstract || type.IsInterface || type == typeof(IController) || type == typeof(RpcAuthController))
                {
                    continue;
                }
                if (iControllerType.IsAssignableFrom(type))
                {
                    UnitContainer.AddSingletonUnit(type);
                    Logger.LogInformation("AddController : {Controller}", type);
                    continue;
                }
                var rpcController = type.GetCustomAttribute(typeof(RpcControllerAttribute));
                if (rpcController == null)
                    continue;
                UnitContainer.AddSingletonUnit(type);
                Logger.LogInformation("AddController : {Controller}", type);
            }
            return this;
        }

        public NettyServerConfiguration CommandPluginsConfigure(
            Action<IUnitCollectionSpec<ICommandPlugin, INetUnitContext>> action)
        {
            action.Invoke(NetUnitContext.CommandPluginSpecs);
            return this;
        }

        public NettyServerConfiguration EndpointConfigure(Action<IEndpointSpec> action)
        {
            action.Invoke(NetUnitContext.EndpointSpec);
            return this;
        }

        public NettyServerConfiguration AuthenticateValidatorsConfigure(
            Action<IUnitCollectionSpec<IAuthenticationValidator, INetUnitContext>> action)
        {
            action.Invoke(NetUnitContext.AuthenticateValidatorSpecs);
            return this;
        }

        public NettyServerConfiguration AddAuthenticateValidators(Action<IUnitSpec<IAuthenticationValidator, INetUnitContext>> action)
        {
            NetUnitContext.AuthenticateValidatorSpecs.AddSpec(action);
            return this;
        }

        public NettyServerConfiguration RegisterConfigurator(ICustomServerConfiguration configuration)
        {
            configurators.Add(configuration);
            return this;
        }

        public NettyServerConfiguration RegisterConfigurator(Action<NettyServerConfiguration> action)
        {
            configurators.Add(new ActionCustomServerConfiguration(action));
            return this;
        }

        public NettyServerConfiguration Initialize()
        {
            if (initialized)
                return this;
            NetUnitContext?.Load();
            foreach (var customServerConfigurator in configurators)
            {
                customServerConfigurator.Configure(this);
            }
            foreach (var builder in netServerGuideBuilders)
            {
                var guide = builder.BuildGuide();
                guides.Add(guide);
                UnitContainer.AddSingletonUnit(guide.Name, guide);
            }

            UnitContainer.BindSingleton(provider => new NetApplication(provider, guides));
            initialized = true;
            return this;
        }
    }

}
