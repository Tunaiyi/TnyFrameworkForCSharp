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

        public IDoneResult<RpcAccessIdentify> Authenticate(long id, string password)
        {
            var identify = RpcAccessIdentify.Parse(id);
            return rpcUserPasswordManager.Auth(identify, password)
                ? DoneResults.Success(identify)
                : DoneResults.Failure<RpcAccessIdentify>(NetResultCode.VALIDATOR_FAIL_ERROR);
        }

        public string CreateToken(RpcServiceType serviceType, RpcAccessIdentify user)
        {
            var token = new RpcAccessToken(serviceType, netAppContext.ServerId, user);
            return JsonConvert.SerializeObject(token);
        }

        public IDoneResult<IRpcToken> VerifyToken(string token)
        {
            var rpcToken = JsonConvert.DeserializeObject<RpcAccessToken>(token);
            return DoneResults.Success(rpcToken);
        }
    }

}
