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
        Message = CodecConstants.DATA_PACK_OPTION_MESSAGE,

        /// <summary>
        /// Ping
        /// </summary>
        Ping = CodecConstants.DATA_PACK_OPTION_PING,

        /// <summary>
        /// Pong
        /// </summary>
        Pong = CodecConstants.DATA_PACK_OPTION_PONG,
    }

    public static class MessageTypeExtensions
    {
        public static byte GetOption(this MessageType value)
        {
            return (byte) value;
        }
    }

}
