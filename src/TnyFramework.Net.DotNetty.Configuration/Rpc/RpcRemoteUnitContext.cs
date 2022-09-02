using Microsoft.Extensions.DependencyInjection;
using TnyFramework.DI.Extensions;
using TnyFramework.DI.Units;
using TnyFramework.Net.Rpc.Remote;

namespace TnyFramework.Net.DotNetty.Configuration.Rpc
{

    public class RpcRemoteUnitContext : IRpcRemoteUnitContext
    {
        private IServiceCollection UnitContainer { get; }

        public UnitSpec<RpcRemoteSetting, IRpcRemoteUnitContext> RpcRemoteSettingSpec { get; }

        public UnitSpec<IRpcRouter, IRpcRemoteUnitContext> DefaultRpcRemoteRouterSpec { get; }

        public RpcRemoteUnitContext(IServiceCollection unitContainer)
        {
            UnitContainer = unitContainer;
            DefaultRpcRemoteRouterSpec = UnitSpec.Unit<IRpcRouter, IRpcRemoteUnitContext>()
                .Default<FirstRpcRouter>();
            RpcRemoteSettingSpec = UnitSpec.Unit<RpcRemoteSetting, IRpcRemoteUnitContext>()
                .Default<RpcRemoteSetting>();
            unitContainer.BindSingleton<RpcRemoteInstanceFactory>();
            unitContainer.BindSingleton(provider => {
                IRpcRouter defaultRpcRouter = null;
                if (DefaultRpcRemoteRouterSpec != null)
                {
                    defaultRpcRouter = DefaultRpcRemoteRouterSpec.Load(this, UnitContainer);
                }
                return new RpcRemoteRouteManager(defaultRpcRouter, provider.GetServices<IRpcRouter>());
            });

        }

        public void Load()
        {
            DefaultRpcRemoteRouterSpec.Load(this, UnitContainer);
            RpcRemoteSettingSpec.Load(this, UnitContainer);
        }
    }

}
