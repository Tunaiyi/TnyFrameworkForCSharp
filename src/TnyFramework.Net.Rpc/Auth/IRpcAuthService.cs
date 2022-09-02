using TnyFramework.Common.Result;

namespace TnyFramework.Net.Rpc.Auth
{

    public interface IRpcAuthService
    {
        IDoneResult<RpcAccessIdentify> Authenticate(long id, string password);

        string CreateToken(RpcServiceType serviceType, RpcAccessIdentify user);

        IDoneResult<RpcAccessToken> VerifyToken(string token);
    }

}
