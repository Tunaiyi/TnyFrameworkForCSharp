#region

using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TnyFramework.Common.Assemblies;
using TnyFramework.Common.Logger;
using TnyFramework.DI.Container;
using TnyFramework.DI.Extensions;
using TnyFramework.DI.Units;
using TnyFramework.Net.Command.Processor;
using TnyFramework.Net.Dispatcher;
using TnyFramework.Net.DotNetty.Bootstrap;
using TnyFramework.Net.DotNetty.Configuration.Endpoint;
using TnyFramework.Net.DotNetty.Configuration.Guide;
using TnyFramework.Net.Plugin;
using TnyFramework.Net.Rpc;
using TnyFramework.Net.Rpc.Attributes;
using TnyFramework.Net.Rpc.Auth;

#endregion

namespace TnyFramework.Net.DotNetty.Configuration
{
    

    public class NettyServerConfiguration : INettyServerConfiguration
    {
        private static readonly ILogger LOGGER = LogFactory.Logger<NettyServerConfiguration>();

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


        public NettyServerConfiguration AppContext(long serverId, string name)
        {
            NetUnitContext.AppContextSpec
                .ServerId(serverId)
                .AppName(name);
            return this;
        }


        public NettyServerConfiguration AppContext(long serverId, string name, string appType, string scope)
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
            LOGGER.LogInformation("AddController : {}", typeof(TController));
            return this;
        }


        public NettyServerConfiguration AddController(IController controller)
        {
            UnitContainer.AddSingletonUnit(controller);
            LOGGER.LogInformation("AddController : {}", controller.GetType());
            return this;
        }


        public NettyServerConfiguration AddController(Func<IServiceProvider, IController> factory)
        {
            throw new NotImplementedException();
        }


        public NettyServerConfiguration AddController<TController>(Func<IServiceProvider, TController> factory)
            where TController : IController
        {
            UnitContainer.AddSingletonUnit(factory);
            LOGGER.LogInformation("AddController : {}", typeof(TController));
            return this;
        }


        public NettyServerConfiguration AddControllers()
        {
            return AddControllers(AssemblyUtils.LoadAllAssemblies());
        }


        public NettyServerConfiguration AddControllers(ICollection<Assembly> assemblies)
        {
            var iControllerType = typeof(IController);
            foreach (var assembly in AssemblyUtils.LoadAllAssemblies(assemblies))
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (type.IsAbstract || type.IsInterface || type == typeof(IController) || type == typeof(RpcAuthController))
                    {
                        continue;
                    }
                    if (iControllerType.IsAssignableFrom(type))
                    {
                        UnitContainer.AddSingletonUnit(type);
                        LOGGER.LogInformation("AddController : {}", type);
                        continue;
                    }
                    var rpcController = type.GetCustomAttribute(typeof(RpcControllerAttribute));
                    if (rpcController != null)
                    {
                        UnitContainer.AddSingletonUnit(type);
                        LOGGER.LogInformation("AddController : {}", type);
                    }
                }
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
            Action<IUnitCollectionSpec<IAuthenticateValidator, INetUnitContext>> action)
        {
            action.Invoke(NetUnitContext.AuthenticateValidatorSpecs);
            return this;
        }


        public NettyServerConfiguration AddAuthenticateValidators(Action<IUnitSpec<IAuthenticateValidator, INetUnitContext>> action)
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
