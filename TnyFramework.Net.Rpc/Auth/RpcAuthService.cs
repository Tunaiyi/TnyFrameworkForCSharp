using Newtonsoft.Json;
using TnyFramework.Common.Result;
using TnyFramework.Net.Base;
using TnyFramework.Net.Common;
namespace TnyFramework.Net.Rpc.Auth
{
    public class RpcAuthService : IRpcAuthService
    {
        private readonly INetAppContext netAppContext;

        private readonly IRpcUserPasswordManager rpcUserPasswordManager;


        public RpcAuthService(INetAppContext netAppContext, IRpcUserPasswordManager rpcUserPasswordManager)
        {
            this.netAppContext = netAppContext;
            this.rpcUserPasswordManager = rpcUserPasswordManager;
        }


        public IDoneResult<IRpcLinkerId> Authenticate(string service, long serverId, long instance, string password)
        {
            return rpcUserPasswordManager.Auth(service, serverId, instance, password)
                ? DoneResults.Success(new RpcLinkerId(service, serverId, instance))
                : DoneResults.Failure<RpcLinkerId>(NetResultCode.VALIDATOR_FAIL_ERROR);
        }


        public string CreateToken(string serviceName, IRpcLinkerId id)
        {
            var token = new RpcToken(serviceName, netAppContext.ServerId, id.Id, id);
            return JsonConvert.SerializeObject(token);
        }


        public IDoneResult<IRpcToken> VerifyToken(string token)
        {
            var rpcToken = JsonConvert.DeserializeObject<RpcToken>(token);
            return DoneResults.Success(rpcToken);
        }
    }
}
