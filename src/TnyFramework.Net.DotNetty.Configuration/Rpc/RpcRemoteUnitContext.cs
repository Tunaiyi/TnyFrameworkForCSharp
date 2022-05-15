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

        public UnitSpec<IRpcRemoteRouter, IRpcRemoteUnitContext> DefaultRpcRemoteRouterSpec { get; }

        public RpcRemoteUnitContext(IServiceCollection unitContainer)
        {
            UnitContainer = unitContainer;
            DefaultRpcRemoteRouterSpec = UnitSpec.Unit<IRpcRemoteRouter, IRpcRemoteUnitContext>()
                .Default<FirstRpcRemoteRouter>();
            RpcRemoteSettingSpec = UnitSpec.Unit<RpcRemoteSetting, IRpcRemoteUnitContext>()
                .Default<RpcRemoteSetting>();
            unitContainer.BindSingleton<RpcRemoteInstanceFactory>();
            unitContainer.BindSingleton(provider => {
                IRpcRemoteRouter defaultRpcRemoteRouter = null;
                if (DefaultRpcRemoteRouterSpec != null)
                {
                    defaultRpcRemoteRouter = DefaultRpcRemoteRouterSpec.Load(this, UnitContainer);
                }
                return new RpcRemoteRouteManager(defaultRpcRemoteRouter, provider.GetServices<IRpcRemoteRouter>());
            });

        }

        public void Load()
        {
            DefaultRpcRemoteRouterSpec.Load(this, UnitContainer);
            RpcRemoteSettingSpec.Load(this, UnitContainer);
        }
    }

}
