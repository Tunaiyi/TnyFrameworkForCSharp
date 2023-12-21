// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TnyFramework.Common.Extensions;
using TnyFramework.Common.Logger;
using TnyFramework.DI.Extensions;
using TnyFramework.DI.Units;
using TnyFramework.Net.Application;
using TnyFramework.Net.Attributes;
using TnyFramework.Net.Command.Auth;
using TnyFramework.Net.Command.Dispatcher;
using TnyFramework.Net.Command.Tasks;
using TnyFramework.Net.Hosting.App;
using TnyFramework.Net.Hosting.Endpoint;
using TnyFramework.Net.Hosting.Guide;
using TnyFramework.Net.Hosting.Selectors;
using TnyFramework.Net.Plugin;
using TnyFramework.Net.Rpc.Auth;

namespace TnyFramework.Net.Hosting.Configuration
{

    public abstract class NetHostConfiguration<TConfiguration, TContext, TGuide, TSpec> : INetHostConfiguration<TConfiguration>
        where TGuide : INetServerGuide
        where TContext : INetGuideUnitContext
        where TConfiguration : INetHostConfiguration<TConfiguration>
        where TSpec : INetGuideSpec<TGuide, TContext, TSpec>
    {
        private readonly ILogger logger;

        private readonly IList<INetServerGuideBuilder<TGuide>> netServerGuideBuilders = new List<INetServerGuideBuilder<TGuide>>();

        private readonly IList<ICustomServerConfiguration<TConfiguration>> serverConfigurations =
            new List<ICustomServerConfiguration<TConfiguration>>();

        private readonly IList<TGuide> guides = new List<TGuide>();

        private bool initialized;

        protected NetUnitContext NetUnitContext { get; }

        protected IServiceCollection UnitContainer { get; }

        protected TConfiguration Self { get; }

        protected NetHostConfiguration(IServiceCollection unitContainer)
        {
            if (this is TConfiguration configuration)
            {
                Self = configuration;
            } else
            {
                throw new InvalidCastException($"{GetType()} cast {nameof(TConfiguration)} exception");
            }
            logger = LogFactory.Logger(GetType());
            UnitContainer = unitContainer;
            NetUnitContext = new NetUnitContext(unitContainer);
        }

        protected abstract TSpec CreateServerGuideSpec(string name);

        public TConfiguration Server(string name, Action<TSpec> action)
        {
            var spec = CreateServerGuideSpec(name);
            action(spec);
            netServerGuideBuilders.Add(spec);
            return Self;
        }

        public TConfiguration AppContext(int serverId, string name)
        {
            NetUnitContext.AppContextSpec
                .ServerId(serverId)
                .AppName(name);
            return Self;
        }

        public TConfiguration AppContext(int serverId, string name, string appType, string scope)
        {
            NetUnitContext.AppContextSpec
                .ServerId(serverId)
                .AppName(name)
                .AppType(appType)
                .ScopeType(scope);
            return Self;
        }

        public TConfiguration AppContextConfigure(Action<INetAppContextSpec> action)
        {
            action.Invoke(NetUnitContext.AppContextSpec);
            return Self;
        }

        public TConfiguration MessageDispatcherConfigure(
            Action<IUnitSpec<IMessageDispatcher, INetUnitContext>> action)
        {
            action.Invoke(NetUnitContext.MessageDispatcherSpec);
            return Self;
        }

        public TConfiguration CommandBoxFactory(
            Action<IUnitSpec<ICommandBoxFactory, INetUnitContext>> action)
        {
            action.Invoke(NetUnitContext.CommandBoxFactorySpec);
            return Self;
        }

        public TConfiguration AddController<TController>() where TController : class, IController
        {
            UnitContainer.AddSingletonUnit<TController>();
            logger.LogInformation("AddController : {Controller}", typeof(TController));
            return Self;
        }

        public TConfiguration AddController(IController controller)
        {
            UnitContainer.AddSingletonUnit(controller);
            logger.LogInformation("AddController : {Controller}", controller.GetType());
            return Self;
        }

        public TConfiguration AddController(Func<IServiceProvider, IController> factory)
        {
            UnitContainer.AddSingletonUnit(factory);
            return Self;
        }

        public TConfiguration AddController<TController>(Func<IServiceProvider, TController> factory)
            where TController : IController
        {
            UnitContainer.AddSingletonUnit(factory);
            logger.LogInformation("AddController : {Controller}", typeof(TController));
            return Self;
        }

        public TConfiguration AddControllers()
        {
            return AddControllers(RpcTypeSelector.Controllers);
        }

        public TConfiguration AddControllers(ICollection<Assembly> assemblies)
        {
            foreach (var assembly in assemblies)
            {
                AddControllers(assembly.GetTypes());
            }
            return Self;
        }

        private TConfiguration AddControllers(IEnumerable<Type> types)
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
                    logger.LogInformation("AddController : {Controller}", type);
                    continue;
                }
                var rpcController = type.GetCustomAttribute(typeof(RpcControllerAttribute));
                if (rpcController == null)
                    continue;
                UnitContainer.AddSingletonUnit(type);
                logger.LogInformation("AddController : {Controller}", type);
            }
            return Self;
        }

        public TConfiguration CommandPluginsConfigure(
            Action<IUnitCollectionSpec<ICommandPlugin, INetUnitContext>> action)
        {
            action.Invoke(NetUnitContext.CommandPluginSpecs);
            return Self;
        }

        public TConfiguration EndpointConfigure(Action<IEndpointSpec> action)
        {
            action.Invoke(NetUnitContext.EndpointSpec);
            return Self;
        }

        public TConfiguration AuthenticateValidatorsConfigure(
            Action<IUnitCollectionSpec<IAuthenticationValidator, INetUnitContext>> action)
        {
            action.Invoke(NetUnitContext.AuthenticateValidatorSpecs);
            return Self;
        }

        public TConfiguration AddAuthenticateValidators(Action<IUnitSpec<IAuthenticationValidator, INetUnitContext>> action)
        {
            NetUnitContext.AuthenticateValidatorSpecs.AddSpec(action);
            return Self;
        }

        public TConfiguration RegisterConfigurator(ICustomServerConfiguration<TConfiguration> configuration)
        {
            serverConfigurations.Add(configuration);
            return Self;
        }

        public TConfiguration RegisterConfigurator(Action<TConfiguration> action)
        {
            serverConfigurations.Add(new ActionCustomServerConfiguration<TConfiguration>(action));
            return Self;
        }

        public TConfiguration Initialize()
        {
            if (initialized)
                return Self;
            if (NetUnitContext.IsNotNull())
            {
                NetUnitContext.Load();
            }
            foreach (var customServerConfigurator in serverConfigurations)
            {
                customServerConfigurator.Configure(Self);
            }
            foreach (var builder in netServerGuideBuilders)
            {
                var guide = builder.BuildGuide();
                guides.Add(guide);
                UnitContainer.AddSingletonUnit(guide.Name, guide);
            }

            UnitContainer.BindSingleton(provider => new NetApplication(provider, guides.Select(g => (INetServerGuide) g).ToList()));
            initialized = true;
            return Self;
        }
    }

}
