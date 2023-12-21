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
using TnyFramework.Common.Binary.Extensions;
using TnyFramework.Net.DotNetty.Common;
using TnyFramework.Net.Exceptions;
using TnyFramework.Net.Message;

namespace TnyFramework.Net.DotNetty.Codec
{

    public class NetPacketV1Encoder : NetPacketV1Codec, INetPacketEncoder
    {
        private readonly DataPacketV1Setting setting;

        private readonly FastThreadLocal<MemoryAllotCounter> counterThreadLocal =
            new FastThreadLocal<MemoryAllotCounter>();

        public NetPacketV1Encoder(DataPacketV1Setting setting, IMessageCodec messageCodec) :
            base(messageCodec)
        {
            this.setting = setting;
        }

        public NetPacketV1Encoder(IMessageCodec messageCodec, DataPacketV1Setting setting, ICodecVerifier codecVerifier) :
            base(messageCodec, codecVerifier)
        {
            this.setting = setting;
        }

        public static MessageType OfMode(MessageMode self)
        {
            switch (self)
            {
                case MessageMode.Ping:
                    return MessageType.Ping;
                case MessageMode.Pong:
                    return MessageType.Pong;
                case MessageMode.Request:
                case MessageMode.Response:
                case MessageMode.Push:
                default:
                    return MessageType.Message;
            }
        }

        public void EncodeObject(IChannelHandlerContext ctx, IMessage message, IByteBuffer outBuffer)
        {
            var channel = ctx.Channel;
            // 写出包头
            outBuffer.WriteBytes(CodecConstants.MAGICS);
            var messageType = OfMode(message.Mode);
            // 非消息, Ping or Pong
            if (messageType != MessageType.Message)
            {
                // 直接写出返回
                outBuffer.WriteByte(messageType.GetOption());
                return;
            }
            // 
            var packageContext = channel.GetAttribute(NettyNetAttrKeys.WRITE_PACKAGER).Get();
            if (packageContext == null)
            {
                var tunnel = channel.GetAttribute(NettyNetAttrKeys.TUNNEL).Get();
                packageContext = new DataPackageContext(tunnel.AccessId, setting);
                channel.GetAttribute(NettyNetAttrKeys.WRITE_PACKAGER).Set(packageContext);
            }
            WritePayload(packageContext, message, outBuffer);
        }

        private void WritePayload(DataPackageContext writePkgContext, IMessage message, IByteBuffer outBuffer)
        {
            IByteBuffer? bodyBuffer = null;
            var counter = Counter();
            var allotSize = -1;
            var actualSize = 0;
            try
            {
                // 写入 Option
                var messageType = OfMode(message.Mode);
                var option = messageType.GetOption();
                option = CodecConstants.SetOption(option, CodecConstants.DATA_PACK_OPTION_VERIFY, setting.VerifyEnable);
                option = CodecConstants.SetOption(option, CodecConstants.DATA_PACK_OPTION_ENCRYPT,
                    setting.EncryptEnable);
                option = CodecConstants.SetOption(option, CodecConstants.DATA_PACK_OPTION_WASTE_BYTES,
                    setting.WasteBytesEnable);
                outBuffer.WriteByte(option);
                //payloadLength
                var payloadLength = 0;
                var accessId = writePkgContext.AccessId;
                payloadLength += VariantExtensions.ComputeVarInt64Len(accessId);
                var number = writePkgContext.NextNumber();
                payloadLength += VariantExtensions.ComputeVarInt32Len(number);
                if (setting.WasteBytesEnable)
                {
                    // TODO 生成废字节长度
                }
                allotSize = counter.Allot();
                bodyBuffer = outBuffer.Allocator.Buffer(allotSize);
                messageCodec.Encode((INetMessage) message, bodyBuffer);
                if (setting.VerifyEnable)
                {
                    // TODO 生成校验 & 字节长度
                }
                if (setting.EncryptEnable)
                {
                    // TODO 加密
                }
                payloadLength += bodyBuffer.ReadableBytes;
                // 判断长度
                if (payloadLength > CodecConstants.PAYLOAD_BYTES_MAX_SIZE)
                {
                    throw new NetException($"payload length {payloadLength} > {CodecConstants.PAYLOAD_BYTES_MAX_SIZE}");
                }
                // 写入包长度
                outBuffer.WriteFixed32(payloadLength);
                // 写入 accessId
                outBuffer.WriteVariant(accessId);
                // 写入 number
                outBuffer.WriteVariant(number);
                // TODO 写入废字节
                // TODO 写入校验码
                // 写入包体
                actualSize = bodyBuffer.ReadableBytes;
                outBuffer.WriteBytes(bodyBuffer);
            } finally
            {
                if (bodyBuffer != null && allotSize > 0)
                {
                    counter.Recode(allotSize, actualSize);
                    ReferenceCountUtil.Release(bodyBuffer);
                }
            }
        }

        private MemoryAllotCounter Counter()
        {
            var counter = counterThreadLocal.Value;
            if (counter == null)
            {
                counterThreadLocal.Value = new MemoryAllotCounter();
            }
            return counterThreadLocal.Value;
        }
    }

}
