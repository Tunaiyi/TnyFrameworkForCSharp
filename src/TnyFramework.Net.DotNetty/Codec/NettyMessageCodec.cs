using System.Collections.Generic;
using System.Collections.Immutable;
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
        private static readonly ILogger LOGGER = LogFactory.Logger<NettyMessageCodec>();

        private readonly IMessageBodyCodec bodyCoder;

        private readonly IMessageHeaderCodec headersCodec;

        private readonly IMessageRelayStrategy messageRelayStrategy = NeverRelayStrategy.Strategy;

        public NettyMessageCodec(IMessageBodyCodec bodyCoder, IMessageHeaderCodec headersCodec)
        {
            this.headersCodec = headersCodec;
            this.bodyCoder = bodyCoder;
        }

        public INetMessage Decode(IByteBuffer buffer, IMessageFactory factory)
        {
            ByteBufferUtils.ReadVariant(buffer, out long id);
            var option = buffer.ReadByte();
            var mode = (MessageMode) (option & CodecConstants.MESSAGE_HEAD_OPTION_MODE_MASK);
            ByteBufferUtils.ReadVariant(buffer, out int protocol);
            ByteBufferUtils.ReadVariant(buffer, out int code);
            ByteBufferUtils.ReadVariant(buffer, out long toMessage);
            ByteBufferUtils.ReadVariant(buffer, out long time);

            int line = (byte) (option & CodecConstants.MESSAGE_HEAD_OPTION_LINE_MASK);
            line >>= CodecConstants.MESSAGE_HEAD_OPTION_LINE_SHIFT;
            IDictionary<string, MessageHeader> headerMap = ImmutableDictionary<string, MessageHeader>.Empty;
            if (CodecConstants.IsOption(option, CodecConstants.MESSAGE_HEAD_OPTION_EXIST_HEADERS_VALUE_EXIST))
            {
                headerMap = ReadHeaders(buffer);
            }
            var head = new CommonMessageHead(id, mode, line, protocol, code, toMessage, time, headerMap);
            if (!CodecConstants.IsOption(option, CodecConstants.MESSAGE_HEAD_OPTION_EXIST_BODY))
                return factory.Create(head, null);
            var body = ReadBody(buffer, messageRelayStrategy.IsRelay(head));
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
            var hasHeader = message.IsHasHeaders;
            var option = mode.GetOption();
            option = (byte) (option |
                             (message.ExistBody ? CodecConstants.MESSAGE_HEAD_OPTION_EXIST_BODY : (byte) 0) |
                             (hasHeader ? CodecConstants.MESSAGE_HEAD_OPTION_EXIST_HEADERS_VALUE_EXIST : (byte) 0));
            var line = head.Line;
            if (line < CodecConstants.MESSAGE_HEAD_OPTION_LINE_MIN_VALUE || line > CodecConstants.MESSAGE_HEAD_OPTION_LINE_MAX_VALUE)
            {
                throw NetCodecException.CauseEncodeFailed(
                    $"line is {line}. line must {CodecConstants.MESSAGE_HEAD_OPTION_LINE_MIN_VALUE} <= line <= {CodecConstants.MESSAGE_HEAD_OPTION_LINE_MAX_VALUE}");
            }
            option = (byte) (option | line << CodecConstants.MESSAGE_HEAD_OPTION_LINE_SHIFT);
            buffer.WriteByte(option);
            ByteBufferUtils.WriteVariant(head.ProtocolId, buffer);
            ByteBufferUtils.WriteVariant(head.Code, buffer);
            ByteBufferUtils.WriteVariant(head.ToMessage, buffer);
            ByteBufferUtils.WriteVariant(head.Time, buffer);
            if (hasHeader)
            {
                WriteHeaders(buffer, message.GetAllHeaders());
            }
            if (!message.ExistBody)
                return;
            var body = message.Body;
            WriteObject(buffer, body, this.bodyCoder);
        }

        private IDictionary<string, MessageHeader> ReadHeaders(IByteBuffer buffer)
        {
            var headerMap = new Dictionary<string, MessageHeader>();
            ByteBufferUtils.ReadVariant(buffer, out int size);
            for (var index = 0; index < size; index++)
            {
                try
                {
                    var header = headersCodec.Decode(buffer);
                    if (header != null)
                    {
                        headerMap[header.Key] = header;
                    }
                } catch (System.Exception e)
                {
                    LOGGER.LogError(e, "decode header exception");
                }
            }
            return headerMap;
        }

        private object ReadBody(IByteBuffer buffer, bool relay)
        {
            object body;
            ByteBufferUtils.ReadVariant(buffer, out int length);
            var bodyBuff = buffer.Allocator.HeapBuffer(length);
            buffer.ReadBytes(bodyBuff, length);
            if (relay)
            {
                // 不释放, 等待转发后释放
                body = new ByteBufferMessageBody(bodyBuff, true);
            } else
            {
                try
                {
                    body = bodyCoder.Decode(bodyBuff);
                } finally
                {
                    ReferenceCountUtil.Release(bodyBuff);
                }
            }
            return body;
        }

        private void WriteHeaders(IByteBuffer buffer, IList<MessageHeader> headers)
        {
            ByteBufferUtils.WriteVariant(headers.Count, buffer);
            foreach (var header in headers)
            {
                try
                {
                    headersCodec.Encode(header, buffer);
                } catch (System.Exception e)
                {
                    LOGGER.LogError(e, $"encode header {header} exception");
                }
            }
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
