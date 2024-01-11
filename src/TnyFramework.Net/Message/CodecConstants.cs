// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System.Linq;

namespace TnyFramework.Net.Message;

public static class CodecConstants
{
    //时间 + MAGICS + data package
    //data package = head option + payloadLength + payload
    //payload = message head option + message

    //magics
    public static readonly byte[] MAGICS = {(byte) 't', (byte) 'n', (byte) 'y', (byte) '.'}; //魔法字节，用于识别
    public static readonly int MAGICS_SIZE = MAGICS.Length; //魔法字节长度

    //package option
    public const byte DATA_PACK_OPTION_BYTES_SIZE = 1; // package option 字节数
    public const byte DATA_PACK_OPTION_MSG_TYPE_SIZE = 2; // message type 字节数

    public const byte
        DATA_PACK_OPTION_MSG_TYPE_MASK = (byte) ~(-1 << DATA_PACK_OPTION_MSG_TYPE_SIZE); // message type 掩码

    public const byte DATA_PACK_OPTION_MESSAGE = 0; // message
    public const byte DATA_PACK_OPTION_PING = 1 << 0; // ping
    public const byte DATA_PACK_OPTION_PONG = 1 << 1; // pong
    public const byte DATA_PACK_OPTION_VERIFY = 1 << 2; // 选项(是否有校验)
    public const byte DATA_PACK_OPTION_ENCRYPT = 1 << 3; // 选项(是否有加密)
    public const byte DATA_PACK_OPTION_WASTE_BYTES = 1 << 4; // 选项(是否有废字节)

    //pay load
    public const byte PAYLOAD_LENGTH_BYTES_SIZE = 4; // 负载长度 字节数
    public const int PAYLOAD_BYTES_MAX_SIZE = 1024 * 1024; // 负载字节列表最大值

    //message head option
    public const byte MESSAGE_HEAD_OPTION_MODE_BIT_SIZE = 2; // message mode 占据位数

    public const byte
        MESSAGE_HEAD_OPTION_MODE_MASK = ~(-1 << MESSAGE_HEAD_OPTION_MODE_BIT_SIZE); // 获取消息模式掩码 00000011

    public const byte MESSAGE_HEAD_OPTION_MODE_VALUE_PING = 0xFF; // ping mode
    public const byte MESSAGE_HEAD_OPTION_MODE_VALUE_PONG = 0xFF - 1; // pong mode
    public const byte MESSAGE_HEAD_OPTION_MODE_VALUE_REQUEST = 0; // request mode
    public const byte MESSAGE_HEAD_OPTION_MODE_VALUE_RESPONSE = 1; // response
    public const byte MESSAGE_HEAD_OPTION_MODE_VALUE_PUSH = 2; // push

    //body
    public const byte MESSAGE_HEAD_OPTION_EXIST_BODY_BIT_SIZE = 1; //是否有body 占据位数
    public const byte MESSAGE_HEAD_OPTION_EXIST_BODY_SHIFT = 2; // body 偏移值
    public const byte MESSAGE_HEAD_OPTION_EXIST_BODY = 1 << MESSAGE_HEAD_OPTION_EXIST_BODY_SHIFT; // 是否有body
    public const byte MESSAGE_HEAD_OPTION_EXIST_HEADERS_VALUE_EXIST = 1 << 6; // 是否请求数据

    //line 消息线路
    public const byte MESSAGE_HEAD_OPTION_LINE_MASK = 0xFF >> 4 << 2; // line 掩码 00011100
    public const byte MESSAGE_HEAD_OPTION_LINE_BIT_SIZE = 3; // line 占据位数

    public const byte MESSAGE_HEAD_OPTION_LINE_SHIFT =
        MESSAGE_HEAD_OPTION_MODE_BIT_SIZE + MESSAGE_HEAD_OPTION_EXIST_BODY_BIT_SIZE; //line 偏移值

    public const byte MESSAGE_HEAD_OPTION_LINE_MIN_VALUE = 0; // line 最小 0

    public const byte
        MESSAGE_HEAD_OPTION_LINE_MAX_VALUE = ~(-1 << MESSAGE_HEAD_OPTION_LINE_BIT_SIZE); // line 最大 00000111

    /// <summary>
    /// 是否是文件头
    /// </summary>
    /// <param name="magics"></param>
    /// <returns></returns>
    public static bool IsMagic(byte[] magics)
    {
        return !magics.Where((t, index) => MAGICS[index] != t).Any();
    }

    /// <summary>
    /// 设置option
    /// </summary>
    /// <param name="option"></param>
    /// <param name="mark"></param>
    /// <param name="effect">mark是否起作用</param>
    /// <returns></returns>
    public static byte SetOption(byte option, byte mark, bool effect)
    {
        if (effect)
        {
            return (byte) (option | mark);
        }
        return option;
    }

    /// <summary>
    /// 判断该option是否是mark
    /// </summary>
    /// <param name="option"></param>
    /// <param name="mark"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool IsOption(byte option, byte mark, byte value)
    {
        return (option & mark) == value;
    }

    /// <summary>
    /// 判断该option是否是mark
    /// </summary>
    /// <param name="option"></param>
    /// <param name="mark"></param>
    /// <returns></returns>
    public static bool IsOption(byte option, byte mark)
    {
        return (option & mark) == mark;
    }
}
