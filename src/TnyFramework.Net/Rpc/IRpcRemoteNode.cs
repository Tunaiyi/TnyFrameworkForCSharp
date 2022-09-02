using System.Collections.Generic;

namespace TnyFramework.Net.Rpc
{

    public interface IRpcRemoteNode : IRpcNode
    {
        /// <summary>
        /// 获取节点上所有 rpc 接入点(连接)的有序列表
        /// </summary>
        /// <returns>返回接入点(连接)的有序列表</returns>
        IList<IRpcRemoteAccess> GetOrderRemoteAccesses();

        /// <summary>
        /// 按照 AccessId 获取指定接入点
        /// </summary>
        /// <param name="accessId">接入点id</param>
        /// <returns>返回指定接入点</returns>
        IRpcRemoteAccess GetRemoteAccess(long accessId);

        /// <summary>
        /// 节点是否活跃(存在有存活的接入点)
        /// </summary>
        /// <returns>返回是否活跃</returns>
        bool IsActive();
    }

}
