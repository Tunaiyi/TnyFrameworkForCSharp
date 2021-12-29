using DotNetty.Buffers;
using DotNetty.Common;
using DotNetty.Common.Utilities;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.Logging;
using TnyFramework.Common.Logger;
using TnyFramework.Net.DotNetty.Common;
using TnyFramework.Net.DotNetty.Exception;
using TnyFramework.Net.DotNetty.Message;
namespace TnyFramework.Net.DotNetty.Codec
{
    public class DatagramPackV1Decoder : DatagramPackV1Codec, IDatagramPackDecoder
    {
        private static readonly ILogger LOGGER = LogFactory.Logger<DatagramPackV1Decoder>();

        private readonly DataPacketV1Config config;

        private static readonly FastThreadLocal<byte[]> MAGICS_LOCAL = new FastThreadLocal<byte[]>();


        public DatagramPackV1Decoder(DataPacketV1Config config, IMessageCodec messageCodec) :
            base(messageCodec)
        {
            this.config = config;
        }


        public DatagramPackV1Decoder(DataPacketV1Config config, IMessageCodec messageCodec, ICodecVerifier codecVerifier) :
            base(messageCodec, codecVerifier)
        {
            this.config = config;
        }


        public byte[] MagicsBuffer => MAGICS_LOCAL.Value ?? (MAGICS_LOCAL.Value = new byte[CodecConstants.MAGICS_SIZE]);


        public object DecodeObject(IChannelHandlerContext ctx, IByteBuffer inBuffer, DatagramPackDecodeMarker marker)
        {
            var channel = ctx.Channel;
            byte option;
            int payloadLength;
            if (marker.Mark)
            {
                option = marker.Option();
                payloadLength = marker.PayloadLength();
            } else
            {
                // 获取打包器
                if (inBuffer.ReadableBytes < CodecConstants.MAGICS_SIZE + CodecConstants.DATA_PACK_OPTION_BYTES_SIZE +
                    CodecConstants.PAYLOAD_LENGTH_BYTES_SIZE)
                {
                    return null;
                }
                var magics = MagicsBuffer;
                inBuffer.ReadBytes(magics);
                if (!CodecConstants.IsMagic(magics))
                {
                    inBuffer.SkipBytes(inBuffer.ReadableBytes);
                    throw NetCodecException.CauseDecodeError("illegal magics");
                }
                option = inBuffer.ReadByte();
                if (CodecConstants.IsOption(option, CodecConstants.DATA_PACK_OPTION_MSG_TYPE_MASK,
                        CodecConstants.MESSAGE_HEAD_OPTION_MODE_VALUE_PING))
                {
                    return TickMessage.Ping();
                }
                if (CodecConstants.IsOption(option, CodecConstants.DATA_PACK_OPTION_MSG_TYPE_MASK,
                        CodecConstants.MESSAGE_HEAD_OPTION_MODE_VALUE_PONG))
                {
                    return TickMessage.Pong();
                }

                ByteBufferUtils.ReadFixed32(inBuffer, out payloadLength);
                if (payloadLength > config.MaxPayloadLength)
                {
                    inBuffer.SkipBytes(inBuffer.ReadableBytes);
                    throw NetCodecException.CauseDecodeError(
                        $"decode message failed, because payloadLength {payloadLength} > maxPayloadLength {config.MaxPayloadLength}");
                }
                marker.Record(option, payloadLength);
            }
            // 读取请求信息体
            if (inBuffer.ReadableBytes < payloadLength)
            {
                return null;
            }
            try
            {
                var message = ReadPayload(channel, inBuffer, option, payloadLength);
                // NetLogger.logReceive(()->channel.attr(NettyNetAttrKeys.TUNNEL).get(), message);
                return message;
            } finally
            {
                marker.Reset();
            }
        }


        private IMessage ReadPayload(IAttributeMap channel, IByteBuffer inBuffer, byte option, int payloadLength)
        {
            IByteBuffer bodyBuffer = null;
            try
            {
                var tunnel = channel.GetAttribute(NettyNetAttrKeys.TUNNEL).Get();
                // 获取打包器
                var index = inBuffer.ReaderIndex;
                ByteBufferUtils.ReadVariant(inBuffer, out long accessId);
                var packageContext = channel.GetAttribute(NettyNetAttrKeys.READ_PACKAGER).Get();
                if (packageContext == null)
                {
                    packageContext = new DataPackageContext(accessId, config);
                    tunnel.SetAccessId(accessId);
                    channel.GetAttribute(NettyNetAttrKeys.READ_PACKAGER).Set(packageContext);
                }
                ByteBufferUtils.ReadVariant(inBuffer, out int number);
                // 移动到当前包序号
                packageContext.GoToAndCheck(number);
                var verifyEnable = CodecConstants.IsOption(option, CodecConstants.DATA_PACK_OPTION_VERIFY);
                if (config.VerifyEnable && !verifyEnable)
                {
                    throw NetCodecException.CauseDecodeError("packet need verify!");
                }
                var encryptEnable = CodecConstants.IsOption(option, CodecConstants.DATA_PACK_OPTION_ENCRYPT);
                if (config.EncryptEnable && !encryptEnable)
                {
                    throw NetCodecException.CauseDecodeError("packet need encrypt!");
                }
                var wasteBytesEnable = CodecConstants.IsOption(option, CodecConstants.DATA_PACK_OPTION_WASTE_BYTES);
                if (config.WasteBytesEnable && !wasteBytesEnable)
                {
                    throw NetCodecException.CauseDecodeError("packet need waste bytes!");
                }
                //        // 检测时间
                //        packager.checkPacketTime(time);
                //计算 body length
                // NettyWasteReader reader = new NettyWasteReader(packageContext, wasteBytesEnable, config);
                var verifyLength = verifyEnable ? codecVerifier.CodeLength : 0;
                var bodyLength = payloadLength - verifyLength - (inBuffer.ReaderIndex - index);
                // 读取废字节中的 bodyBytes
                bodyBuffer = inBuffer.Allocator.HeapBuffer(bodyLength);

                LOGGER.LogDebug("in payloadIndex start {}", inBuffer.ReaderIndex);
                inBuffer.ReadBytes(bodyBuffer, bodyLength);

                LOGGER.LogDebug("in payloadIndex end {}", inBuffer.ReaderIndex);
                // 加密
                if (encryptEnable)
                {
                    // this.crypto.decrypt(packageContext, bodyBuffer.array(), bodyBuffer.arrayOffset(),
                    //     bodyBuffer.readableBytes());
                    // if (logger.isDebugEnabled())
                    // {
                    //     CodecLogger.logBinary(logger, "sendMessage body decryption |  body  {} ",
                    //         bodyBuffer.array(), bodyBuffer.arrayOffset(), bodyBuffer.readableBytes());
                    // }
                }
                // 校验码验证
                if (verifyEnable)
                {
                    var verifyCode = new byte[verifyLength];
                    inBuffer.ReadBytes(verifyCode);
                    // if (this.verifier.verify(packageContext, bodyBuffer.array(), bodyBuffer.arrayOffset(),
                    //         bodyBuffer.readableBytes(), verifyCode))
                    // {
                    //     throw NetCodecException.CauseVerify("packet verify failed");
                    // }
                }

                return messageCodec.Decode(bodyBuffer, factory: tunnel.MessageFactory);
            } finally
            {
                ReferenceCountUtil.Release(bodyBuffer);
            }
        }
    }
}
