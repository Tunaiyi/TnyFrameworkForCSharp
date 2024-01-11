// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using DotNetty.Buffers;
using TnyFramework.Net.Message;

namespace TnyFramework.Net.DotNetty.Codec;

/// <summary>
/// IMessage 编解码器
/// </summary>
public interface IMessageCodec
{
    /// <summary>
    /// 编码器
    /// </summary>
    /// <param name="message">编码消息</param>
    /// <param name="buffer">写出字节 buffer</param>
    void Encode(INetMessage message, IByteBuffer buffer);

    /// <summary>
    /// 解码器
    /// </summary>
    /// <param name="buffer">读取字节 buffer</param>
    /// <param name="factory">消息工厂</param>
    /// <returns>返回解析消息</returns>
    INetMessage Decode(IByteBuffer buffer, IMessageFactory factory);
}
