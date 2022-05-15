using System.Collections.Generic;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.Logging;
using TnyFramework.Common.Logger;
using TnyFramework.Net.Message;

namespace TnyFramework.Net.DotNetty.Codec
{

    public class NetPacketDecodeHandler : ByteToMessageDecoder
    {
        private static readonly ILogger LOGGER = LogFactory.Logger<NetPacketDecodeHandler>();

        private readonly INetPacketDecoder decoder;

        private readonly bool closeOnError;

        private readonly NetPacketDecodeMarker marker = new NetPacketDecodeMarker();

        public NetPacketDecodeHandler(INetPacketDecoder decoder, bool closeOnError)
        {
            this.decoder = decoder;
            this.closeOnError = closeOnError;
        }

        protected override void Decode(IChannelHandlerContext context, IByteBuffer input, List<object> output)
        {
            try
            {
                while (input.ReadableBytes > 0)
                {
                    var messageObject = decoder.DecodeObject(context, input, marker);
                    if (messageObject is IMessage message)
                    {
                        output.Add(message);
                    } else
                    {
                        break;
                    }
                }
            } catch (System.Exception exception)
            {
                NetPacketCodecErrorHandler.HandleOnEncodeError(LOGGER, context, exception, closeOnError);
            }
        }
    }

}
