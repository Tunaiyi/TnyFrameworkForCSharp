using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using TnyFramework.Net.Message;

namespace TnyFramework.Net.DotNetty.Codec
{

    /// <summary>
    /// 序列化Message 接口
    /// </summary>
    public interface INetPacketEncoder
    {
        /// <summary>
        /// 编码Message
        /// </summary>
        /// <param name="ctx">通道上下问</param>
        /// <param name="message">消息</param>
        /// <param name="outBuffer">写出 buffer</param> 
        void EncodeObject(IChannelHandlerContext ctx, IMessage message, IByteBuffer outBuffer);
    }

}
