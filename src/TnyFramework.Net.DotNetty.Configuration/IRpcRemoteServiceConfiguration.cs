using System;
using System.Collections.Generic;
using System.Reflection;
using TnyFramework.DI.Units;
using TnyFramework.Net.DotNetty.Configuration.Rpc;
using TnyFramework.Net.Rpc.Remote;

namespace TnyFramework.Net.DotNetty.Configuration
{

    public interface IRpcRemoteServiceConfiguration
    {
        RpcRemoteServiceConfiguration RpcRemoteSettingConfigure(Action<IUnitSpec<RpcRemoteSetting, IRpcRemoteUnitContext>> action);

        RpcRemoteServiceConfiguration AddRemoteService<TRpcRemoteService>() where TRpcRemoteService : class;

        RpcRemoteServiceConfiguration AddRemoteService(Type type);

        RpcRemoteServiceConfiguration AddRemoteServices();

        RpcRemoteServiceConfiguration AddRemoteServices(IEnumerable<Type> types);

        RpcRemoteServiceConfiguration AddRemoteServices(ICollection<Assembly> assemblies);
    }

}
