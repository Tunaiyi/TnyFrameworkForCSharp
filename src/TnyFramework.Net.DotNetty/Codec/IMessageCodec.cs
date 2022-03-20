using DotNetty.Buffers;
using TnyFramework.Net.Message;
namespace TnyFramework.Net.DotNetty.Codec
{
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
}
