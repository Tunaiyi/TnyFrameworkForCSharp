// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;

namespace TnyFramework.Net.Message;

public enum MessageHeaderUsage
{
    /// <summary>
    /// 临时
    /// </summary>
    Transient = 1,

    /// <summary>
    /// 本地传递
    /// </summary>
    LocalFeedback = 2,

    /// <summary>
    /// 本地传递
    /// </summary>
    LocalInfect = 3,

    /// <summary>
    /// 远程返回
    /// </summary>
    RemoteFeedback = 4,

    /// <summary>
    /// 远程传染
    /// </summary>
    RemoteInfect = 5,
}

public static class MessageHeaderUsageExtensions
{
    /// <summary>
    /// 是否传递到关联Message
    /// </summary>
    /// <param name="usage"></param>
    /// <param name="returnMode"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static bool IsInherit(this MessageHeaderUsage usage, MessageMode returnMode)
    {
        switch (returnMode)
        {
            case MessageMode.Request:
                return usage is MessageHeaderUsage.LocalFeedback or MessageHeaderUsage.RemoteFeedback or MessageHeaderUsage.RemoteInfect;
            case MessageMode.Response:
                return usage is MessageHeaderUsage.LocalFeedback or MessageHeaderUsage.RemoteFeedback or MessageHeaderUsage.RemoteInfect;
            case MessageMode.Push:
                return usage is MessageHeaderUsage.LocalFeedback or MessageHeaderUsage.RemoteInfect;
            case MessageMode.Ping:
            case MessageMode.Pong:
                return false;
            default:
                throw new ArgumentOutOfRangeException(nameof(returnMode), returnMode, null);
        }
    }

    /// <summary>
    /// 是否用于传送
    /// </summary>
    /// <param name="usage"></param>
    /// <returns></returns>
    public static bool IsTransmissive(this MessageHeaderUsage usage)
    {
        switch (usage)
        {
            case MessageHeaderUsage.RemoteFeedback:
            case MessageHeaderUsage.RemoteInfect:
                return true;
            case MessageHeaderUsage.Transient:
            case MessageHeaderUsage.LocalInfect:
            case MessageHeaderUsage.LocalFeedback:
            default:
                return false;
        }
    }
}
