using System;
using TnyFramework.Common.Exception;
using TnyFramework.Net.Message;
using TnyFramework.Net.Transport;
namespace TnyFramework.Net.Rpc.Auth
{
    public static class RpcAuthMessageContexts
    {
        public const int RPC_AUTH_SERVICE_INDEX = 0;

        public const int RPC_AUTH_SERVER_ID_INDEX = 1;

        public const int RPC_AUTH_INSTANCE_INDEX = 2;

        public const int RPC_AUTH_PASSWORD_INDEX = 3;


        public static RequestContext AuthRequest(string service, long serverId, long instance, string password)
        {
            return MessageContexts.Request(Protocols.Protocol(RpcProtocol.RPC_AUTH_4_AUTHENTICATE),
                service, serverId, instance, password);
        }


        public static string GetServiceParam(MessageParamList paramList)
        {
            if (paramList.Get<string>(RPC_AUTH_SERVICE_INDEX, out var value))
            {
                return value;
            }
            throw new IllegalArgumentException($"index {RPC_AUTH_SERVICE_INDEX} service param is null");
        }


        public static long GetServerIdParam(MessageParamList paramList)
        {
            if (paramList.Get<long>(RPC_AUTH_SERVER_ID_INDEX, out var value))
            {
                return value;
            }
            throw new IllegalArgumentException($"index {RPC_AUTH_SERVER_ID_INDEX} service id param is null");
        }


        public static long GetInstanceIdParam(MessageParamList paramList)
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
