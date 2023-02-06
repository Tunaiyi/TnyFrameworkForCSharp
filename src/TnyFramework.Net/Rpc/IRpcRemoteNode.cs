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

    public interface IRpcRemoteNode : IRpcNode
    {
        /// <summary>
        /// 获取节点上所有 rpc 接入点(连接)的有序列表
        /// </summary>
        /// <returns>返回接入点(连接)的有序列表</returns>
        IList<IRpcAccess> GetOrderRemoteAccesses();

        /// <summary>
        /// 按照 AccessId 获取指定接入点
        /// </summary>
        /// <param name="accessId">接入点id</param>
        /// <returns>返回指定接入点</returns>
        IRpcAccess GetRemoteAccess(long accessId);

        /// <summary>
        /// 节点是否活跃(存在有存活的接入点)
        /// </summary>
        /// <returns>返回是否活跃</returns>
        bool IsActive();
    }

}
