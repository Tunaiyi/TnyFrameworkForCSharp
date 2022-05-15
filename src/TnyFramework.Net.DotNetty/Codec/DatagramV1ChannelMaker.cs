namespace TnyFramework.Net.DotNetty.Codec
{

    public class DatagramV1ChannelMaker : DatagramChannelMaker
    {
        public DatagramV1ChannelMaker(INetPacketEncoder encoder, bool closeOnEncodeError, INetPacketDecoder decoder, bool closeOnDecodeError) :
            base(encoder, closeOnEncodeError, decoder, closeOnDecodeError)
        {
        }

        public DatagramV1ChannelMaker(DataPacketV1Setting setting, IMessageCodec messageCodec, bool closeOnEncodeError, bool closeOnDecodeError)
            : base(new NetPacketV1Encoder(setting, messageCodec), closeOnEncodeError,
                new NetPacketV1Decoder(setting, messageCodec), closeOnDecodeError)
        {
        }
    }

}
