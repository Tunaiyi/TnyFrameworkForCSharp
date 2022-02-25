using System.Collections.Generic;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.Logging;
using TnyFramework.Common.Logger;
using TnyFramework.Net.Message;
namespace TnyFramework.Net.DotNetty.Codec
{
    public class DatagramPackDecodeHandler : ByteToMessageDecoder
    {
        private static readonly ILogger LOGGER = LogFactory.Logger<DatagramPackDecodeHandler>();

        private readonly IDatagramPackDecoder decoder;

        private readonly bool closeOnError;

        private readonly DatagramPackDecodeMarker marker = new DatagramPackDecodeMarker();


        public DatagramPackDecodeHandler(IDatagramPackDecoder decoder, bool closeOnError)
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
                DatagramPackCodecErrorHandler.HandleOnEncodeError(LOGGER, context, exception, closeOnError);
            }
        }
    }
}
