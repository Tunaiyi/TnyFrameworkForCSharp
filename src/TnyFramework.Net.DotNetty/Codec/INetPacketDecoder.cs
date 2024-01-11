// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using DotNetty.Buffers;
using DotNetty.Transport.Channels;

namespace TnyFramework.Net.DotNetty.Codec;

/// <summary>
/// 反序列化Message 接口
/// </summary>
public interface INetPacketDecoder
{
    /// <summary>
    /// 解析Message
    /// </summary>
    /// <param name="ctx">通道上下问</param>
    /// <param name="inBuffer">写出通道</param>
    /// <param name="marker">数据包标记器</param>
    /// <returns></returns>
    object? DecodeObject(IChannelHandlerContext ctx, IByteBuffer inBuffer, NetPacketDecodeMarker marker);
}
