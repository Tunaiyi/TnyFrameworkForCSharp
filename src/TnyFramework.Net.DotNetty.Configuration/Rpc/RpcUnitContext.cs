using Microsoft.Extensions.DependencyInjection;
using TnyFramework.Net.DotNetty.Configuration.Guide;
using TnyFramework.Net.Rpc.Auth;

namespace TnyFramework.Net.DotNetty.Configuration.Rpc
{

    public class RpcUnitContext : IRpcUnitContext
    {
        private IServiceCollection UnitContainer { get; }

        public NetUnitContext NetUnitContext { get; }

        public RpcAuthServiceSpec RpcAuthServiceSpec { get; }

        public RpcUnitContext(NetUnitContext netUnitContext, IServiceCollection unitContainer)
        {
            UnitContainer = unitContainer;
            NetUnitContext = netUnitContext;
            RpcAuthServiceSpec = new RpcAuthServiceSpec(this, unitContainer);
        }

        public IRpcAuthService LoadRpcAuthService()
        {
            return RpcAuthServiceSpec.Load(this, UnitContainer);
        }

        public IRpcUserPasswordManager LoadRpcUserPasswordManager()
        {
            return RpcAuthServiceSpec.LoadRpcUserPasswordManager();
        }
    }

}
