using DotNetty.Transport.Channels;

namespace TnyFramework.Net.DotNetty.Codec
{

    public class DatagramChannelMaker : ChannelMaker<IChannel>
    {
        private readonly INetPacketEncoder encoder;

        private readonly bool closeOnEncodeError;

        private readonly INetPacketDecoder decoder;

        private readonly bool closeOnDecodeError;

        public DatagramChannelMaker(INetPacketEncoder encoder, bool closeOnEncodeError, INetPacketDecoder decoder, bool closeOnDecodeError)
        {
            this.encoder = encoder;
            this.closeOnEncodeError = closeOnEncodeError;
            this.decoder = decoder;
            this.closeOnDecodeError = closeOnDecodeError;
        }

        protected override void MakeChannel(IChannel channel)
        {
            var channelPipeline = channel.Pipeline;
            channelPipeline.AddLast("frameDecoder", new NetPacketDecodeHandler(decoder, closeOnDecodeError));
            channelPipeline.AddLast("encoder", new NetPacketEncodeHandler(encoder, closeOnEncodeError));
        }

        protected override void PostInitChannel(IChannel channel)
        {
        }
    }

}
