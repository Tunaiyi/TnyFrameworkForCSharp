using TnyFramework.Common.Exception;
using TnyFramework.Net.Message;
using TnyFramework.Net.Transport;

namespace TnyFramework.Net.Rpc.Auth
{

    public static class RpcAuthMessageContexts
    {
        public const int RPC_AUTH_INSTANCE_INDEX = 0;

        public const int RPC_AUTH_PASSWORD_INDEX = 1;


        public static RequestContext AuthRequest(long id, string password)
        {
            return MessageContexts.Request(Protocols.Protocol(RpcProtocol.RPC_AUTH_4_AUTHENTICATE), id, password);
        }


        public static long GetIdParam(MessageParamList paramList)
        {
            if (paramList.Get<long>(RPC_AUTH_INSTANCE_INDEX, out var value))
            {
                return value;
            }
            throw new IllegalArgumentException($"index {RPC_AUTH_INSTANCE_INDEX} service id param is null");
        }


        public static string GetPasswordParam(MessageParamList paramList)
        {
            if (paramList.Get(RPC_AUTH_PASSWORD_INDEX, "", out var value))
            {
                return value;
            }
            throw new IllegalArgumentException($"index {RPC_AUTH_PASSWORD_INDEX} service id param is null");
        }
    }

}
