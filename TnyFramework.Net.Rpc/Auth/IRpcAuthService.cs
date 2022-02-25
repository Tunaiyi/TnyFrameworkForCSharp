using TnyFramework.Common.Result;
namespace TnyFramework.Net.Rpc.Auth
{
    public interface IRpcAuthService
    {
        IDoneResult<IRpcLinkerId> Authenticate(string service, long serverId, long instance, string password);

        string CreateToken(string serviceName, IRpcLinkerId id);

        IDoneResult<IRpcToken> VerifyToken(string token);
    }
}
