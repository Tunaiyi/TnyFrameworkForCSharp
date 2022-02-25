using System;
using Microsoft.Extensions.DependencyInjection;
using TnyFramework.DI.Units;
using TnyFramework.Net.Rpc.Auth;
namespace TnyFramework.Net.DotNetty.Configuration.Rpc
{
    public interface IRpcAuthServiceSpec : IUnitSpec<IRpcAuthService, IRpcUnitContext>
    {
        RpcAuthServiceSpec RpcUserPasswordManagerConfigure(Action<UnitSpec<IRpcUserPasswordManager, IRpcUnitContext>> action);
    }

    public class RpcAuthServiceSpec : UnitSpec<IRpcAuthService, IRpcUnitContext>, IRpcAuthServiceSpec
    {
        private IServiceCollection UnitContainer { get; }

        private IRpcUnitContext RpcUnitContext { get; }

        private readonly UnitSpec<IRpcUserPasswordManager, IRpcUnitContext> rpcUserPasswordManagerSpec;


        public RpcAuthServiceSpec(IRpcUnitContext rpcUnitContext, IServiceCollection container)
        {
            RpcUnitContext = rpcUnitContext;
            UnitContainer = container;
            Default(DefaultRpcAuthService);
            rpcUserPasswordManagerSpec = UnitSpec.Unit<IRpcUserPasswordManager, IRpcUnitContext>()
                .Default<NoopRpcUserPasswordManager>();
        }


        public RpcAuthServiceSpec RpcUserPasswordManagerConfigure(Action<UnitSpec<IRpcUserPasswordManager, IRpcUnitContext>> action)
        {
            action.Invoke(rpcUserPasswordManagerSpec);
            return this;
        }


        public IRpcAuthService DefaultRpcAuthService(IRpcUnitContext context)
        {
            var netUnitContext = context.NetUnitContext;
            return new RpcAuthService(netUnitContext.LoadAppContext(), context.LoadRpcUserPasswordManager());
        }


        public IRpcUserPasswordManager LoadRpcUserPasswordManager()
        {
            return rpcUserPasswordManagerSpec.Load(RpcUnitContext, UnitContainer);
        }
    }
}
