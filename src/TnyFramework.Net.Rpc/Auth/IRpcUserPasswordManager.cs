namespace TnyFramework.Net.Rpc.Auth
{
    public interface IRpcUserPasswordManager
    {
        /// <summary>
        /// 认证
        /// </summary>
        /// <param name="service">服务名</param>
        /// <param name="serverId">服务 id</param>
        /// <param name="instance">实例 id</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        bool Auth(string service, long serverId, long instance, string password);
    }
}
