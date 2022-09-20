// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

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
