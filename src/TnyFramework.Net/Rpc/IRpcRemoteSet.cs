using System.Collections.Generic;

namespace TnyFramework.Net.Rpc
{

    public interface IRpcRemoteSet
    {
        /// <summary>
        /// 获取有序远程节点列表
        /// </summary>
        IList<RpcRemoteServiceNode> OrderRemoteNodes { get; }

        /// <summary>
        /// 查找远程节点
        /// </summary>
        /// <param name="nodeId">节点id</param>
        /// <returns></returns>
        IRpcRemoteNode FindRemoteNode(int nodeId);

        /// <summary>
        /// 查找远程接入(连接)
        /// </summary>
        /// <param name="nodeId">节点id</param>
        /// <param name="accessId">接入点id</param>
        /// <returns></returns>
        IRpcRemoteAccess FindRemoteAccess(int nodeId, long accessId);
    }

}
