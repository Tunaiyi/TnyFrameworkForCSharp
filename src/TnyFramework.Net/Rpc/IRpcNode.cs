namespace TnyFramework.Net.Rpc
{

    public interface IRpcNode
    {
        /// <summary>
        /// 节点id
        /// </summary>
        int NodeId { get; }

        /// <summary>
        /// 服务类型
        /// </summary>
        IRpcServiceType ServiceType { get; }
    }

}
