using System.Collections.Generic;
using TnyFramework.Net.Base;
using TnyFramework.Net.Command.Processor;
using TnyFramework.Net.Dispatcher;
using TnyFramework.Net.DotNetty.Configuration.Endpoint;
using TnyFramework.Net.Plugin;
using TnyFramework.Net.Rpc;

namespace TnyFramework.Net.DotNetty.Configuration.Guide
{

    public interface INetUnitContext
    {
        void Load();

        MessageDispatcherContext LoadMessageDispatcherContext();

        IMessageDispatcher LoadMessageDispatcher();

        ICommandTaskBoxProcessor LoadCommandTaskProcessor();

        IEndpointUnitContext EndpointUnitContext { get; }

        INetAppContext LoadAppContext();

        IRpcRemoteServiceManager LoadRpcRemoteServiceManager();

        IList<ICommandPlugin> LoadCommandPlugins();

        IList<IAuthenticateValidator> LoadAuthenticateValidators();
    }

}
