using DotNetty.Transport.Channels;
namespace TnyFramework.Net.DotNetty.Codec
{
    public class DatagramChannelMaker : BaseChannelMaker<IChannel>
    {
        private readonly IDatagramPackEncoder encoder;

        private readonly bool closeOnEncodeError;

        private readonly IDatagramPackDecoder decoder;

        private readonly bool closeOnDecodeError;


        public DatagramChannelMaker(IDatagramPackEncoder encoder, bool closeOnEncodeError, IDatagramPackDecoder decoder, bool closeOnDecodeError)
        {
            this.encoder = encoder;
            this.closeOnEncodeError = closeOnEncodeError;
            this.decoder = decoder;
            this.closeOnDecodeError = closeOnDecodeError;
        }


        protected override void MakeChannel(IChannel channel)
        {
            var channelPipeline = channel.Pipeline;
            channelPipeline.AddLast("frameDecoder", new DatagramPackDecodeHandler(decoder, closeOnDecodeError));
            channelPipeline.AddLast("encoder", new DatagramPackEncodeHandler(encoder, closeOnEncodeError));
        }


        protected override void PostInitChannel(IChannel channel)
        {
        }
    }
}
