// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System.Collections.Generic;

namespace TnyFramework.Net.Rpc;

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
