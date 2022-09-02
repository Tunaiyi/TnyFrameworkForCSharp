namespace TnyFramework.Net.Rpc
{

    public interface IRpcForwardAccess : IRpcRemoteAccess
    {
        /// <summary>
        /// 服务 id
        /// </summary>
        RpcAccessIdentify Identify { get; }

        /// <summary>
        /// 获取转发服务者
        /// </summary>
        ForwardPoint ForwardPoint { get; }
    }

}
