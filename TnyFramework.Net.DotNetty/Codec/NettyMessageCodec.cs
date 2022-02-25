using DotNetty.Buffers;
using DotNetty.Common.Utilities;
using Microsoft.Extensions.Logging;
using TnyFramework.Common.Logger;
using TnyFramework.Net.DotNetty.Common;
using TnyFramework.Net.DotNetty.Exception;
using TnyFramework.Net.Message;
namespace TnyFramework.Net.DotNetty.Codec
{
    public class NettyMessageCodec : IMessageCodec
    {
        public static readonly ILogger LOGGER = LogFactory.Logger<NettyMessageCodec>();

        private readonly IMessageBodyCodec bodyCoder;

        private readonly IMessageRelayStrategy messageRelayStrategy = NeverRelayStrategy.Strategy;


        public NettyMessageCodec(IMessageBodyCodec bodyCoder)
        {
            this.bodyCoder = bodyCoder;
        }



        public INetMessage Decode(IByteBuffer buffer, IMessageFactory factory)
        {
            ByteBufferUtils.ReadVariant(buffer, out long id);
            var option = buffer.ReadByte();
            var mode = (MessageMode)(option & CodecConstants.MESSAGE_HEAD_OPTION_MODE_MASK);
            ByteBufferUtils.ReadVariant(buffer, out int protocol);
            ByteBufferUtils.ReadVariant(buffer, out int code);
            ByteBufferUtils.ReadVariant(buffer, out long toMessage);
            ByteBufferUtils.ReadVariant(buffer, out long time);
            object body = null;
            int line = (byte)(option & CodecConstants.MESSAGE_HEAD_OPTION_LINE_MASK);
            line >>= CodecConstants.MESSAGE_HEAD_OPTION_LINE_SHIFT;
            var head = new CommonMessageHead(id, mode, line, protocol, code, toMessage, time);
            if (!CodecConstants.IsOption(option, CodecConstants.MESSAGE_HEAD_OPTION_EXIST_BODY))
                return factory.Create(head, null);
            ByteBufferUtils.ReadVariant(buffer, out int length);

            LOGGER.LogTrace("read {} body length {}", head, length);

            var bodyBuff = buffer.Allocator.HeapBuffer(length);
            if (messageRelayStrategy.IsRelay(head))
            {
                // WARN 不释放, 等待转发后释放
                buffer.ReadBytes(bodyBuff, length);
                body = new ByteBufferMessageBody(bodyBuff, true);
            } else
            {
                buffer.ReadBytes(bodyBuff, length);
                try
                {
                    body = bodyCoder.Decode(bodyBuff);
                } finally
                {
                    ReferenceCountUtil.Release(bodyBuff);
                }
            }
            return factory.Create(head, body);
        }


        public void Encode(INetMessage message, IByteBuffer buffer)
        {
            if (message.Type != MessageType.Message)
            {
                return;
            }
            //		ProtoExOutputStream stream = new ProtoExOutputStream(1024, 2 * 1024);
            ByteBufferUtils.WriteVariant(message.Id, buffer);
            var head = message.Head;
            var mode = head.Mode;
            var option = mode.GetOption();
            option = (byte)(option | (message.ExistBody ? CodecConstants.MESSAGE_HEAD_OPTION_EXIST_BODY : (byte)0));
            var line = head.Line;
            if (line < CodecConstants.MESSAGE_HEAD_OPTION_LINE_MIN_VALUE || line > CodecConstants.MESSAGE_HEAD_OPTION_LINE_MAX_VALUE)
            {
                throw NetCodecException.CauseEncodeFailed(
                    $"line is {line}. line must {CodecConstants.MESSAGE_HEAD_OPTION_LINE_MIN_VALUE} <= line <= {CodecConstants.MESSAGE_HEAD_OPTION_LINE_MAX_VALUE}");
            }
            option = (byte)(option | line << CodecConstants.MESSAGE_HEAD_OPTION_LINE_SHIFT);
            buffer.WriteByte(option);
            ByteBufferUtils.WriteVariant(head.ProtocolId, buffer);
            ByteBufferUtils.WriteVariant(head.Code, buffer);
            ByteBufferUtils.WriteVariant(head.ToMessage, buffer);
            ByteBufferUtils.WriteVariant(head.Time, buffer);
            if (!message.ExistBody)
                return;
            var body = message.Body;
            WriteObject(buffer, body, this.bodyCoder);
        }


        private static void WriteObject(IByteBuffer buffer, object body, IMessageBodyCodec coder)
        {

            IOctetMessageBody releaseBody = null;
            try
            {
                switch (body)
                {
                    case byte[] array:
                        Write(buffer, array);
                        break;
                    case ByteArrayMessageBody arrayBody:
                        releaseBody = arrayBody;
                        Write(buffer, arrayBody.Body);
                        break;
                    case ByteBufferMessageBody bufferBody: {
                        releaseBody = bufferBody;
                        var data = bufferBody.Body;
                        if (data == null)
                        {
                            throw NetCodecException.CauseEncodeFailed("ByteBufMessageBody is released");
                        }
                        ByteBufferUtils.WriteVariant(data.ReadableBytes, buffer);
                        buffer.WriteBytes(data);
                        break;
                    }
                    default: {
                        IByteBuffer bodyBuf = null;
                        try
                        {
                            bodyBuf = buffer.Allocator.HeapBuffer();
                            coder.Encode(body, bodyBuf);
                            ByteBufferUtils.WriteVariant(bodyBuf.ReadableBytes, buffer);
                            buffer.WriteBytes(bodyBuf);
                        } finally
                        {
                            ReferenceCountUtil.Release(bodyBuf);
                        }
                        break;
                    }
                }
            } finally
            {
                releaseBody?.Release();
            }
        }


        private static void Write(IByteBuffer buffer, byte[] data)
        {
            ByteBufferUtils.WriteVariant(data.Length, buffer);
            buffer.WriteBytes(data);
        }
    }
}
