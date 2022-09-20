// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;

namespace TnyFramework.Net.Message
{

    [Flags]
    public enum MessageType
    {
        /// <summary>
        /// 处理请求
        /// </summary>
        Message = 0,

        /// <summary>
        /// Ping
        /// </summary>
        Ping = 1,

        /// <summary>
        /// Pong
        /// </summary>
        Pong = 2,
    }

    public static class MessageTypeExtensions
    {
        public static byte GetOption(this MessageType value)
        {
            return (byte) value;
        }
    }

}
