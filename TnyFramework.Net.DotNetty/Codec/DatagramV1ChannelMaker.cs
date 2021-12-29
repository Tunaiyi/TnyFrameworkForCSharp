using DotNetty.Transport.Channels;
namespace TnyFramework.Net.DotNetty.Codec
{
    public class DatagramV1ChannelMaker : DatagramChannelMaker
    {
        public DatagramV1ChannelMaker(DataPacketV1Config config, IMessageCodec messageCodec, bool closeOnEncodeError, bool closeOnDecodeError)
            : base(new DatagramPackV1Encoder(config, messageCodec), closeOnEncodeError,
                new DatagramPackV1Decoder(config, messageCodec), closeOnDecodeError)
        {
        }
    }
}
