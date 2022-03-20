using System;
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
        public IRpcResult<string> Authenticate(IServerBootstrapSetting setting, [UserId] IRpcLinkerId id)
        {
            var token = rpcAuthService.CreateToken(setting.ServiceName(), id);
            return RpcResults.Success(token);
        }


        [RpcResponse(RpcProtocol.RPC_AUTH_4_AUTHENTICATE)]
        [AuthenticationRequired(typeof(RpcTokenValidator))]
        public void Authenticated([UserId] RpcLinkerId id)
        {
        }
    }
}
