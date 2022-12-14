// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System.Collections.Generic;

namespace TnyFramework.Net.Message
{

    /// <summary>
    /// 消息内容接口
    /// </summary>
    public interface IMessageContent : IMessageSchema
    {
        /// <summary>
        /// 是否存在消息
        /// </summary>
        bool ExistBody { get; }

        /// <summary>
        /// 获取消息体
        /// </summary>
        object Body { get; }

        /// <summary>
        /// 获取所有的 header
        /// </summary>
        /// <returns></returns>
        IDictionary<string, MessageHeader> Headers { get; }

        /// <summary>
        /// 获取消息体
        /// </summary>
        /// <typeparam name="T">消息体类型</typeparam>
        /// <returns></returns>
        T BodyAs<T>();

        /// <summary>
        /// 结果码
        /// </summary>
        int GetCode();
    }

}
