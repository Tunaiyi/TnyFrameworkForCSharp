// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System.Threading.Tasks;
using TnyFramework.Net.Message;

namespace TnyFramework.Net.Transport
{

    public interface ISender
    {
        /// <summary>
        /// 异步发送消息
        /// </summary>
        /// <param name="content">发送消息上下文</param>
        /// <param name="waitWritten">是否等待</param>
        /// <returns>返回发送回执</returns>
        ValueTask<IMessageSent> Send(MessageContent content, bool waitWritten = false);
    }

}
