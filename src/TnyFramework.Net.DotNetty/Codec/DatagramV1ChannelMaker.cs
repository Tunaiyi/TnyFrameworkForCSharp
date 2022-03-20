namespace TnyFramework.Net.DotNetty.Codec
{
    public class DatagramV1ChannelMaker : DatagramChannelMaker
    {
        public DatagramV1ChannelMaker(IDatagramPackEncoder encoder, bool closeOnEncodeError, IDatagramPackDecoder decoder, bool closeOnDecodeError) :
            base(encoder, closeOnEncodeError, decoder, closeOnDecodeError)
        {
        }


        public DatagramV1ChannelMaker(DataPacketV1Setting setting, IMessageCodec messageCodec, bool closeOnEncodeError, bool closeOnDecodeError)
            : base(new DatagramPackV1Encoder(setting, messageCodec), closeOnEncodeError,
                new DatagramPackV1Decoder(setting, messageCodec), closeOnDecodeError)
        {
        }
    }
}
