namespace TnyFramework.Net.Rpc.Attributes
{

    /// <summary>
    /// 远程调用模式
    /// </summary>
    public enum RpcInvokeMode
    {
        /// <summary>
        /// 默认, 根据返回值
        /// </summary>
        Default,

        /// <summary>
        /// 同步
        /// </summary>
        Sync,

        /// <summary>
        /// 异步
        /// </summary>
        Async,
    }

}
