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
    /// 消息工厂
    /// </summary>
    public interface IMessageFactory
    {
        /// <summary>
        /// 通过 content 创建 message
        /// </summary>
        /// <param name="id">消息 di</param>
        /// <param name="subject"> 内容</param>
        /// <returns>创建的消息</returns>
        INetMessage Create(long id, IMessageSubject subject);

        /// <summary>
        /// 创建 message
        /// </summary>
        /// <param name="head">消息头</param>
        /// <param name="body">消息体</param>
        /// <returns>创建消息</returns>
        INetMessage Create(INetMessageHead head, object? body);
    }

}
