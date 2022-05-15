using DotNetty.Buffers;

namespace TnyFramework.Net.DotNetty.Codec
{

    public interface INetContentCodec
    {
        /// <summary>
        /// 编码
        /// </summary>
        /// <param name="body">消息体</param>
        /// <param name="buffer">写入字节</param>
        void Encode(object body, IByteBuffer buffer);

        /// <summary>
        /// 解码
        /// </summary>
        /// <param name="buffer">读取字节</param>
        /// <returns>解析的消息体</returns>
        object Decode(IByteBuffer buffer);
    }

    public interface INetContentCodec<T> : INetContentCodec
    {
        /// <summary>
        /// 编码
        /// </summary>
        /// <param name="body">消息体</param>
        /// <param name="buffer">写入字节</param>
        void Encode(T body, IByteBuffer buffer);

        /// <summary>
        /// 解码
        /// </summary>
        /// <param name="buffer">读取字节</param>
        /// <returns>解析的消息体</returns>
        new T Decode(IByteBuffer buffer);
    }

}
