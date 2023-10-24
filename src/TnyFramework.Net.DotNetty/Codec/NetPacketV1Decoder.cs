// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using DotNetty.Buffers;
using DotNetty.Common;
using DotNetty.Common.Utilities;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.Logging;
using TnyFramework.Common.Logger;
using TnyFramework.Net.DotNetty.Common;
using TnyFramework.Net.Exceptions;
using TnyFramework.Net.Message;

namespace TnyFramework.Net.DotNetty.Codec
{

    public class NetPacketV1Decoder : NetPacketV1Codec, INetPacketDecoder
    {
        private static readonly ILogger LOGGER = LogFactory.Logger<NetPacketV1Decoder>();

        private readonly DataPacketV1Setting setting;

        private static readonly FastThreadLocal<byte[]> MAGICS_LOCAL = new FastThreadLocal<byte[]>();

        public NetPacketV1Decoder(DataPacketV1Setting setting, IMessageCodec messageCodec) :
            base(messageCodec)
        {
            this.setting = setting;
        }

        public NetPacketV1Decoder(DataPacketV1Setting setting, IMessageCodec messageCodec, ICodecVerifier codecVerifier) :
            base(messageCodec, codecVerifier)
        {
            this.setting = setting;
        }

        public byte[] MagicsBuffer => MAGICS_LOCAL.Value ?? (MAGICS_LOCAL.Value = new byte[CodecConstants.MAGICS_SIZE]);

        public object? DecodeObject(IChannelHandlerContext ctx, IByteBuffer inBuffer, NetPacketDecodeMarker marker)
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
                if (payloadLength > setting.MaxPayloadLength)
                {
                    inBuffer.SkipBytes(inBuffer.ReadableBytes);
                    throw NetCodecException.CauseDecodeError(
                        $"decode message failed, because payloadLength {payloadLength} > maxPayloadLength {setting.MaxPayloadLength}");
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
            IByteBuffer? bodyBuffer = null;
            try
            {
                var tunnel = channel.GetAttribute(NettyNetAttrKeys.TUNNEL).Get();
                // 获取打包器
                var index = inBuffer.ReaderIndex;
                ByteBufferUtils.ReadVariant(inBuffer, out long accessId);
                var packageContext = channel.GetAttribute(NettyNetAttrKeys.READ_PACKAGER).Get();
                if (packageContext == null)
                {
                    packageContext = new DataPackageContext(accessId, setting);
                    tunnel.SetAccessId(accessId);
                    channel.GetAttribute(NettyNetAttrKeys.READ_PACKAGER).Set(packageContext);
                }
                ByteBufferUtils.ReadVariant(inBuffer, out int number);
                // 移动到当前包序号
                packageContext.GoToAndCheck(number);
                var verifyEnable = CodecConstants.IsOption(option, CodecConstants.DATA_PACK_OPTION_VERIFY);
                if (setting.VerifyEnable && !verifyEnable)
                {
                    throw NetCodecException.CauseDecodeError("packet need verify!");
                }
                var encryptEnable = CodecConstants.IsOption(option, CodecConstants.DATA_PACK_OPTION_ENCRYPT);
                if (setting.EncryptEnable && !encryptEnable)
                {
                    throw NetCodecException.CauseDecodeError("packet need encrypt!");
                }
                var wasteBytesEnable = CodecConstants.IsOption(option, CodecConstants.DATA_PACK_OPTION_WASTE_BYTES);
                if (setting.WasteBytesEnable && !wasteBytesEnable)
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

                LOGGER.LogDebug("in payloadIndex start {Index}", inBuffer.ReaderIndex);
                inBuffer.ReadBytes(bodyBuffer, bodyLength);

                LOGGER.LogDebug("in payloadIndex end {Index}", inBuffer.ReaderIndex);
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
