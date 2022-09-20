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
    public enum MessageMode
    {
        /// <summary>
        /// 请求
        /// </summary>
        Request = 0,

        /// <summary>
        /// 响应
        /// </summary>
        Response = 1,

        /// <summary>
        /// 推送
        /// </summary>
        Push = 2,

        /// <summary>
        /// ping
        /// </summary>
        Ping = 0xFF,

        /// <summary>
        /// pong
        /// </summary>
        Pong = 0xFE,
    }

    public static class MessageModeExtensions
    {
        public static byte GetOption(this MessageMode value)
        {
            return (byte) value;
        }

        public static MessageType GetMessageType(this MessageMode self)
        {
            switch (self)
            {
                case MessageMode.Ping:
                    return MessageType.Ping;
                case MessageMode.Pong:
                    return MessageType.Pong;
                case MessageMode.Request:
                case MessageMode.Response:
                case MessageMode.Push:
                default:
                    return MessageType.Message;
            }
        }
    }

}
