namespace TnyFramework.Net.Rpc.Auth
{

    public class NoopRpcUserPasswordManager : IRpcUserPasswordManager
    {
        public bool Auth(RpcAccessIdentify identify, string password)
        {
            return true;
        }
    }

}
