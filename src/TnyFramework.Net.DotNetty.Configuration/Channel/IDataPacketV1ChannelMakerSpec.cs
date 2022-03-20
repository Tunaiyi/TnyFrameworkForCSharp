using System;
using TnyFramework.DI.Units;
using TnyFramework.Net.DotNetty.Codec;
using TnyFramework.Net.DotNetty.Configuration.Guide;
namespace TnyFramework.Net.DotNetty.Configuration.Channel
{
    public interface IDataPacketV1ChannelMakerSpec
    {
        DataPacketV1ChannelMakerSpec CloseOnEncodeError(bool value);

        DataPacketV1ChannelMakerSpec CloseOnDecodeError(bool value);

        DataPacketV1ChannelMakerSpec PackEncodeSetting(Action<IDataPacketV1SettingSpec> action);

        DataPacketV1ChannelMakerSpec PackDecodeSetting(Action<IDataPacketV1SettingSpec> action);

        DataPacketV1ChannelMakerSpec PackEncoder(UnitCreator<IDatagramPackEncoder, INetGuideUnitContext> factory);

        DataPacketV1ChannelMakerSpec PackEncoder(Action<IUnitSpec<IDatagramPackEncoder, INetGuideUnitContext>> action);

        DataPacketV1ChannelMakerSpec PackDecoder(IDatagramPackDecoder decoder);

        DataPacketV1ChannelMakerSpec PackDecoder(UnitCreator<IDatagramPackDecoder, INetGuideUnitContext> factory);

        DataPacketV1ChannelMakerSpec PackDecoder(Action<IUnitSpec<IDatagramPackDecoder, INetGuideUnitContext>> action);

        DataPacketV1ChannelMakerSpec CodecVerifier(ICodecVerifier verifier);

        DataPacketV1ChannelMakerSpec CodecVerifier(UnitCreator<ICodecVerifier, INetGuideUnitContext> factory);

        DataPacketV1ChannelMakerSpec CodecVerifier(Action<IUnitSpec<ICodecVerifier, INetGuideUnitContext>> action);

        DataPacketV1ChannelMakerSpec CodecCrypto(ICodecCrypto crypto);

        DataPacketV1ChannelMakerSpec CodecCrypto(UnitCreator<ICodecCrypto, INetGuideUnitContext> factory);

        DataPacketV1ChannelMakerSpec CodecCrypto(Action<IUnitSpec<ICodecCrypto, INetGuideUnitContext>> action);

        DataPacketV1ChannelMakerSpec ChannelPipelineChains(Action<IUnitCollectionSpec<IChannelPipelineChain, INetGuideUnitContext>> action);
    }
}