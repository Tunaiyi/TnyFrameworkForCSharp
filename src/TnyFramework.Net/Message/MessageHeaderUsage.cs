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

    public enum MessageHeaderUsage
    {
        /// <summary>
        /// 临时
        /// </summary>
        Transient = 1,

        /// <summary>
        /// 传递(单次)
        /// </summary>
        Once = 2,

        /// <summary>
        /// 返回
        /// </summary>
        Feedback = 3,

        /// <summary>
        /// 传染
        /// </summary>
        Infect = 4,
    }

    public static class MessageHeaderUsageExtensions
    {
        public static bool IsTransitive(this MessageHeaderUsage usage, MessageMode messageMode)
        {
            switch (messageMode)
            {
                case MessageMode.Request:
                    return usage is MessageHeaderUsage.Once or MessageHeaderUsage.Feedback or MessageHeaderUsage.Infect;
                case MessageMode.Response:
                    return usage is MessageHeaderUsage.Feedback or MessageHeaderUsage.Infect;
                case MessageMode.Push:
                    return usage is MessageHeaderUsage.Infect;
                case MessageMode.Ping:
                case MessageMode.Pong:
                    return false;
                default:
                    throw new ArgumentOutOfRangeException(nameof(messageMode), messageMode, null);
            }
        }
    }

}
