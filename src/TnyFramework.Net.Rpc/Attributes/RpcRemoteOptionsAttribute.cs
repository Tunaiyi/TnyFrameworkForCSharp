// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using TnyFramework.Net.Rpc.Remote;

namespace TnyFramework.Net.Rpc.Attributes
{

    /// <summary>
    /// 忽略作为远程参数/消息体
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Interface | AttributeTargets.Class)]
    public class RpcRemoteOptionsAttribute : Attribute
    {
        public RpcInvokeMode Mode { get; set; } = RpcInvokeMode.Default;

        /// <summary>
        /// 是否是寂寞方式(不抛出异常)
        /// </summary>
        public bool Silently { get; set; } = false;

        /// <summary>
        /// 超时
        /// -1 为 setting 配置时间,
        /// >= 0 超时
        /// </summary>
        public int Timeout { get; set; } = -1;

        /// <summary>
        /// 路由器类型
        /// </summary>
        public Type Router { get; set; } = typeof(IRpcRouter);
    }

}
