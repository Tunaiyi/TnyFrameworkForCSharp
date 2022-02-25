namespace TnyFramework.Net.Rpc.Auth
{
    public class NoopRpcUserPasswordManager : IRpcUserPasswordManager
    {
        public bool Auth(string service, long serverId, long instance, string password)
        {
            return true;
        }
    }
}
