namespace TnyFramework.Net.Rpc.Auth
{

    public interface IRpcUserPasswordManager
    {
        /// <summary>
        /// 认证
        /// </summary>
        /// <param name="identify">Rpc接入标识</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        bool Auth(RpcAccessIdentify identify, string password);
    }

}
