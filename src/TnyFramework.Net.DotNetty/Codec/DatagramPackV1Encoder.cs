using DotNetty.Buffers;
using DotNetty.Common;
using DotNetty.Common.Utilities;
using DotNetty.Transport.Channels;
using TnyFramework.Net.DotNetty.Common;
using TnyFramework.Net.DotNetty.Exception;
using TnyFramework.Net.Message;
namespace TnyFramework.Net.DotNetty.Codec
{
    public class DatagramPackV1Encoder : DatagramPackV1Codec, IDatagramPackEncoder
    {
        private readonly DataPacketV1Setting setting;

        private readonly FastThreadLocal<MemoryAllotCounter> counterThreadLocal =
            new FastThreadLocal<MemoryAllotCounter>();


        public DatagramPackV1Encoder(DataPacketV1Setting setting, IMessageCodec messageCodec) :
            base(messageCodec)
        {
            this.setting = setting;
        }


        public DatagramPackV1Encoder(IMessageCodec messageCodec, DataPacketV1Setting setting, ICodecVerifier codecVerifier) :
            base(messageCodec, codecVerifier)
        {
            this.setting = setting;
        }


        public void EncodeObject(IChannelHandlerContext ctx, IMessage message, IByteBuffer outBuffer)
        {
            var channel = ctx.Channel;
            // 写出包头
            outBuffer.WriteBytes(CodecConstants.MAGICS);
            var messageType = message.Type;
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
            IByteBuffer bodyBuffer = null;
            var counter = Counter();
            var allotSize = -1;
            var actualSize = 0;
            try
            {
                // 写入 Option
                var option = message.Type.GetOption();
                option = CodecConstants.SetOption(option, CodecConstants.DATA_PACK_OPTION_VERIFY, setting.VerifyEnable);
                option = CodecConstants.SetOption(option, CodecConstants.DATA_PACK_OPTION_ENCRYPT,
                    setting.EncryptEnable);
                option = CodecConstants.SetOption(option, CodecConstants.DATA_PACK_OPTION_WASTE_BYTES,
                    setting.WasteBytesEnable);
                outBuffer.WriteByte(option);
                //payloadLength
                var payloadLength = 0;
                var accessId = writePkgContext.AccessId;
                payloadLength += ByteBufferUtils.ComputeVarInt64Len(accessId);
                var number = writePkgContext.NextNumber();
                payloadLength += ByteBufferUtils.ComputeVarInt32Len(number);
                if (setting.WasteBytesEnable)
                {
                    // TODO 生成废字节长度
                }
                allotSize = counter.Allot();
                bodyBuffer = outBuffer.Allocator.Buffer(allotSize);
                messageCodec.Encode((INetMessage)message, bodyBuffer);
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
                ByteBufferUtils.WriteFixed32(payloadLength, outBuffer);
                // 写入 accessId
                ByteBufferUtils.WriteVariant(accessId, outBuffer);
                // 写入 number
                ByteBufferUtils.WriteVariant(number, outBuffer);
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