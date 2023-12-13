// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace TnyFramework.Net.Message
{

    /// <summary>
    /// 消息结构
    /// </summary>
    public interface IMessageSchema : IProtocol
    {
        /// <summary>
        /// 响应消息, -1 为无
        /// </summary>
        long ToMessage { get; }

        /// <summary>
        /// 消息模式
        /// </summary>
        MessageMode Mode { get; }

    }

}
