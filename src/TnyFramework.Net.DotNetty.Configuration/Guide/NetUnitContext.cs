using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using TnyFramework.DI.Units;
using TnyFramework.Net.Base;
using TnyFramework.Net.Command.Processor;
using TnyFramework.Net.Dispatcher;
using TnyFramework.Net.DotNetty.Configuration.Endpoint;
using TnyFramework.Net.Plugin;
using TnyFramework.Net.Rpc;
namespace TnyFramework.Net.DotNetty.Configuration.Guide
{
    public class NetUnitContext : INetUnitContext
    {
        private IServiceCollection UnitContainer { get; }

        public NetAppContextSpec AppContextSpec { get; }

        public EndpointSpec EndpointSpec { get; }

        public IEndpointUnitContext EndpointUnitContext => EndpointSpec;

        public UnitSpec<MessageDispatcherContext, INetUnitContext> MessageDispatcherContextSpec { get; }

        public UnitSpec<IMessageDispatcher, INetUnitContext> MessageDispatcherSpec { get; }

        public UnitSpec<ICommandTaskBoxProcessor, INetUnitContext> CommandTaskBoxProcessorSpec { get; }

        public UnitCollectionSpec<ICommandPlugin, INetUnitContext> CommandPluginSpecs { get; }

        public UnitCollectionSpec<IAuthenticateValidator, INetUnitContext> AuthenticateValidatorSpecs { get; }


        public NetUnitContext(IServiceCollection unitContainer)
        {
            UnitContainer = unitContainer;


            // MessageDispatcher 
            MessageDispatcherSpec = UnitSpec.Unit<IMessageDispatcher, INetUnitContext>()
                .Default(DefaultMessageDispatcher);

            MessageDispatcherContextSpec = UnitSpec.Unit<MessageDispatcherContext, INetUnitContext>()
                .Default(DefaultMessageDispatcherContext);

            // CommandTaskBoxProcessor
            CommandTaskBoxProcessorSpec = UnitSpec.Unit<ICommandTaskBoxProcessor, INetUnitContext>()
                .Default<CoroutineCommandTaskBoxProcessor>();

            // CommandPlugin
            CommandPluginSpecs = UnitCollectionSpec.Units<ICommandPlugin, INetUnitContext>();

            // AuthenticateValidator
            AuthenticateValidatorSpecs = UnitCollectionSpec.Units<IAuthenticateValidator, INetUnitContext>();

            // Endpoint
            EndpointSpec = new EndpointSpec(UnitContainer);

            // AppContext
            AppContextSpec = new NetAppContextSpec();

        }


        public MessageDispatcherContext LoadMessageDispatcherContext()
        {
            return MessageDispatcherContextSpec.Load(this, UnitContainer);
        }


        public IMessageDispatcher LoadMessageDispatcher()
        {
            return MessageDispatcherSpec.Load(this, UnitContainer);
        }


        public ICommandTaskBoxProcessor LoadCommandTaskProcessor()
        {
            return CommandTaskBoxProcessorSpec.Load(this, UnitContainer);
        }


        public INetAppContext LoadAppContext()
        {
            return AppContextSpec.Load(this, UnitContainer);
        }


        public IList<ICommandPlugin> LoadCommandPlugins()
        {
            return CommandPluginSpecs.Load(this, UnitContainer);
        }


        public IList<IAuthenticateValidator> LoadAuthenticateValidators()
        {
            return AuthenticateValidatorSpecs.Load(this, UnitContainer);
        }


        // private static ISessionKeeperFactory DefaultSessionKeeperFactory<TUserId>(INetUnitContext context)
        // {
        //     var endpointUnitContext = context.EndpointUnitContext;
        //     return new SessionKeeperFactory<TUserId>(endpointUnitContext.LoadSessionFactories());
        // }



        private static MessageDispatcherContext DefaultMessageDispatcherContext(INetUnitContext context)
        {
            return new MessageDispatcherContext(
                context.LoadAppContext(),
                context.LoadCommandPlugins(),
                context.LoadAuthenticateValidators());
        }


        private static MessageDispatcher DefaultMessageDispatcher(INetUnitContext context)
        {
            return new MessageDispatcher(context.LoadMessageDispatcherContext(), context.EndpointUnitContext.LoadEndpointKeeperManager());
        }
    }
}
