using System.Collections.Generic;

namespace TnyFramework.Net.Rpc
{

    public interface IRpcForwardNode : IRpcNode
    {
        /// <summary>
        /// 通过接入 Id 获取接入点
        /// </summary>
        /// <param name="id">接入id</param>
        /// <returns>返回接入点</returns>
        IRpcForwardAccess GetForwardAccess(long id);

        /// <summary>
        /// 获取有序的接入点列表
        /// </summary>
        /// <returns>有序的接入点列表</returns>
        List<RpcForwardAccess> GetOrderForwardAccess();

        /// <summary>
        /// 是否活跃
        /// </summary>
        /// <returns></returns>
        bool IsActive();
    }

}
