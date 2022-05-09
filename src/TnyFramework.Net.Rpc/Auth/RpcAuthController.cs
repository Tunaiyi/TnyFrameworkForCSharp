using TnyFramework.Net.Base;
using TnyFramework.Net.Command;
using TnyFramework.Net.Dispatcher;
using TnyFramework.Net.Rpc.Attributes;

namespace TnyFramework.Net.Rpc.Auth
{

    [RpcController]
    public class RpcAuthController : IController
    {
        private readonly IRpcAuthService rpcAuthService;


        public RpcAuthController(IRpcAuthService rpcAuthService)
        {
            this.rpcAuthService = rpcAuthService;
        }


        [RpcRequest(RpcProtocol.RPC_AUTH_4_AUTHENTICATE)]
        [AuthenticationRequired(typeof(RpcPasswordValidator))]
        public IRpcResult<string> Authenticate(IServerBootstrapSetting setting, [UserId] RpcAccessIdentify id)
        {
            var serviceType = RpcServiceType.ForService(setting.ServiceName());
            var token = rpcAuthService.CreateToken(serviceType, id);
            return RpcResults.Success(token);
        }


        [RpcResponse(RpcProtocol.RPC_AUTH_4_AUTHENTICATE)]
        [AuthenticationRequired(typeof(RpcTokenValidator))]
        public void Authenticated([UserId] RpcAccessIdentify id)
        {
        }
    }

}
