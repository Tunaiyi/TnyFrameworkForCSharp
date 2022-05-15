namespace TnyFramework.Net.Rpc.Remote
{

    /// <summary>
    /// 远程调用配置
    /// </summary>
    public class RpcRemoteSetting
    {
        /// <summary>
        /// 调用超时时间
        /// </summary>
        public int InvokeTimeout { get; set; } = 5000;
    }

}
