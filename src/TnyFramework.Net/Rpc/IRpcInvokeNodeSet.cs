// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System.Collections.Generic;

namespace TnyFramework.Net.Rpc
{

    public interface IRpcInvokeNodeSet
    {
        /// <summary>
        /// 获取有序远程节点列表
        /// </summary>
        /// <returns>节点列表</returns>
        IList<IRpcInvokeNode> GetOrderInvokeNodes();

        /// <summary>
        /// 查找远程节点
        /// </summary>
        /// <param name="nodeId">节点 id</param>
        /// <returns>远程节点</returns>
        IRpcInvokeNode? FindInvokeNode(int nodeId);

        /// <summary>
        /// 查找远程接入(连接)
        /// </summary>
        /// <param name="nodeId">节点 id</param>
        /// <param name="accessId">接入点 id</param>
        /// <returns>远程接入点</returns>
        IRpcAccess? FindInvokeAccess(int nodeId, long accessId);
    }

}
