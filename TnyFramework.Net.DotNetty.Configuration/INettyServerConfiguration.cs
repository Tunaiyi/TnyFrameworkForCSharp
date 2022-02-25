using System;
using TnyFramework.DI.Units;
using TnyFramework.Net.Command.Processor;
using TnyFramework.Net.Dispatcher;
using TnyFramework.Net.DotNetty.Configuration.Endpoint;
using TnyFramework.Net.DotNetty.Configuration.Guide;
using TnyFramework.Net.Plugin;
using TnyFramework.Net.Rpc;
namespace TnyFramework.Net.DotNetty.Configuration
{
    public interface INettyServerConfiguration
    {
        NettyServerConfiguration Server<TUserId>(string name, Action<INetServerGuideSpec<TUserId>> action);

        NettyServerConfiguration AppContext(long serverId, string name);

        NettyServerConfiguration AppContext(long serverId, string name, string appType, string scope);

        NettyServerConfiguration AppContextConfigure(Action<INetAppContextSpec> action);


        NettyServerConfiguration MessageDispatcherConfigure(
            Action<IUnitSpec<IMessageDispatcher, INetUnitContext>> action);


        NettyServerConfiguration CommandTaskBoxProcessor(
            Action<IUnitSpec<ICommandTaskBoxProcessor, INetUnitContext>> action);


        NettyServerConfiguration AddController<TController>() where TController : class, IController;

        NettyServerConfiguration AddController(IController controller);

        NettyServerConfiguration AddController(Func<IServiceProvider, IController> factory);


        NettyServerConfiguration CommandPluginsConfigure(
            Action<IUnitCollectionSpec<ICommandPlugin, INetUnitContext>> action);


        NettyServerConfiguration EndpointConfigure(Action<IEndpointSpec> action);


        NettyServerConfiguration AuthenticateValidatorsConfigure(
            Action<IUnitCollectionSpec<IAuthenticateValidator, INetUnitContext>> action);


        NettyServerConfiguration AddAuthenticateValidators(Action<IUnitSpec<IAuthenticateValidator, INetUnitContext>> action);
    }
}
