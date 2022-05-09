using TnyFramework.Common.Result;

namespace TnyFramework.Net.Rpc.Auth
{

    public interface IRpcAuthService
    {
        IDoneResult<RpcAccessIdentify> Authenticate(long id, string password);

        string CreateToken(RpcServiceType serviceType, RpcAccessIdentify user);

        IDoneResult<IRpcToken> VerifyToken(string token);
    }

}
