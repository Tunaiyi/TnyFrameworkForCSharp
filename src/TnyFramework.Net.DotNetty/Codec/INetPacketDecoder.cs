using DotNetty.Buffers;
using DotNetty.Transport.Channels;

namespace TnyFramework.Net.DotNetty.Codec
{

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
        object DecodeObject(IChannelHandlerContext ctx, IByteBuffer inBuffer, NetPacketDecodeMarker marker);
    }

}
