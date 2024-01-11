// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;

namespace TnyFramework.Net.Message;

[Flags]
public enum MessageMode
{
    /// <summary>
    /// 请求
    /// </summary>
    Request = CodecConstants.MESSAGE_HEAD_OPTION_MODE_VALUE_REQUEST,

    /// <summary>
    /// 响应
    /// </summary>
    Response = CodecConstants.MESSAGE_HEAD_OPTION_MODE_VALUE_RESPONSE,

    /// <summary>
    /// 推送
    /// </summary>
    Push = CodecConstants.MESSAGE_HEAD_OPTION_MODE_VALUE_PUSH,

    /// <summary>
    /// ping
    /// </summary>
    Ping = CodecConstants.MESSAGE_HEAD_OPTION_MODE_VALUE_PING,

    /// <summary>
    /// pong
    /// </summary>
    Pong = CodecConstants.MESSAGE_HEAD_OPTION_MODE_VALUE_PONG,
}

public static class MessageModeExtensions
{
    public static byte GetOption(this MessageMode value)
    {
        return (byte) value;
    }

    public static bool IsHeartBeat(this MessageMode value)
    {
        return value is MessageMode.Ping or MessageMode.Pong;
    }

    public static bool IsMessage(this MessageMode value)
    {
        return value is MessageMode.Request or MessageMode.Response or MessageMode.Push;
    }

    public static int GetIndex(this MessageMode self)
    {
        switch (self)
        {
            case MessageMode.Request:
            case MessageMode.Response:
            case MessageMode.Push:
            default:
                return (int) self;
            case MessageMode.Ping:
                return 3;
            case MessageMode.Pong:
                return 4;
        }
    }

    public static string Mark(this MessageMode self)
    {
        return self.ToString().ToLower();
    }

    public static NetworkWay GetWay(this MessageMode self)
    {
        switch (self)
        {
            case MessageMode.Request:
            case MessageMode.Response:
            case MessageMode.Push:
                return NetworkWay.MESSAGE;
            case MessageMode.Ping:
            case MessageMode.Pong:
                return NetworkWay.HEARTBEAT;
            default:
                throw new ArgumentOutOfRangeException(nameof(self), self, null);
        }
    }
}
