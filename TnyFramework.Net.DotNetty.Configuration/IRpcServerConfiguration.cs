using System;
using TnyFramework.DI.Units;
using TnyFramework.Net.Base;
using TnyFramework.Net.DotNetty.Configuration.Guide;
using TnyFramework.Net.DotNetty.Configuration.Rpc;
using TnyFramework.Net.Rpc.Auth;
using TnyFramework.Net.Transport;
namespace TnyFramework.Net.DotNetty.Configuration
{
    public interface IRpcServerConfiguration : INettyServerConfiguration
    {
        RpcServerConfiguration RpcServer(ServerSetting setting, Action<INetServerGuideSpec<IRpcLinkerId>> action = null);

        RpcServerConfiguration RpcServer(string name, int port, Action<INetServerGuideSpec<IRpcLinkerId>> action = null);

        RpcServerConfiguration RpcServer(string name, string host, int port, Action<INetServerGuideSpec<IRpcLinkerId>> action = null);


        RpcServerConfiguration RpcServer(string name, string host, int port, bool libuv,
            Action<INetServerGuideSpec<IRpcLinkerId>> action = null);


        RpcServerConfiguration RpcServer(string name, string serveName, string host, int port,
            Action<INetServerGuideSpec<IRpcLinkerId>> action = null);


        RpcServerConfiguration RpcServer(string name, string serveName, string host, int port, bool libuv,
            Action<INetServerGuideSpec<IRpcLinkerId>> action = null);


        RpcServerConfiguration RpcAuthServiceSpecConfigure(Action<RpcAuthServiceSpec> action);

        RpcServerConfiguration IdGeneratorConfigure(Action<UnitSpec<IIdGenerator, IRpcUnitContext>> action);
    }
}
