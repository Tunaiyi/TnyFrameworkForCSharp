using System;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.Logging;
using TnyFramework.Common.Logger;
using TnyFramework.Net.DotNetty.Exception;
using TnyFramework.Net.Message;

namespace TnyFramework.Net.DotNetty.Codec
{

    public class NetPacketEncodeHandler : MessageToByteEncoder<object>
    {
        private static readonly ILogger LOGGER = LogFactory.Logger<NetPacketEncodeHandler>();

        private readonly INetPacketEncoder encoder;

        private readonly bool closeOnError;

        public NetPacketEncodeHandler(INetPacketEncoder encoder, bool closeOnError)
        {
            this.encoder = encoder;
            this.closeOnError = closeOnError;
        }

        protected override void Encode(IChannelHandlerContext context, object value, IByteBuffer output)
        {
            switch (value)
            {
                case IByteBuffer buff:
                    output.WriteBytes(buff);
                    return;
                case byte[] bytes:
                    output.WriteBytes(bytes);
                    return;
                case ArraySegment<byte> segment:
                    output.WriteBytes(segment.Array, segment.Offset, segment.Count);
                    return;
                case IMessage message:
                    try
                    {
                        encoder.EncodeObject(context, message, output);
                    } catch (System.Exception e)
                    {
                        NetPacketCodecErrorHandler.HandleOnEncodeError(LOGGER, context, e, closeOnError);
                    }
                    return;
                default:
                    throw NetCodecException.CauseEncodeFailed($"can not encode {value.GetType()}");
            }
        }
    }

}