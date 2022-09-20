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

    public interface IRpcForwardSet
    {
        /// <summary>
        /// 服务类型
        /// </summary>
        RpcServiceType ServiceType { get; }

        /// <summary>
        /// 获取有序的节点列表
        /// </summary>
        /// <returns>有序的节点列表</returns>
        List<IRpcForwardNode> GetOrderForwarderNodes();

        /// <summary>
        /// 查指定服务者的接入点
        /// </summary>
        /// <param name="point">服务点</param>
        /// <returns>返回接入点</returns>
        IRpcRemoteAccess FindForwardAccess(IRpcServicerPoint point);
    }

}
