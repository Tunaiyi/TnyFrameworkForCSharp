// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using TnyFramework.Net.Message;

namespace TnyFramework.Net.Attributes
{

    [AttributeUsage(AttributeTargets.Method)]
    public abstract class RpcProtocolAttribute : Attribute
    {
        protected RpcProtocolAttribute(int protocol, params MessageMode[] modes)
        {
            Protocol = protocol;
            MessageModes = modes;
        }

        protected RpcProtocolAttribute(int protocol, int line, params MessageMode[] modes)
        {
            Protocol = protocol;
            MessageModes = modes;
            Line = line;
        }

        /// <summary>
        /// 协议 id
        /// </summary>
        public int Protocol { get; }

        /// <summary>
        /// 接收线路 id
        /// </summary>
        public int Line { get; } = 0;

        /// <summary>
        /// 处理消息类型
        /// </summary>
        public MessageMode[] MessageModes { get; }
    }

}
