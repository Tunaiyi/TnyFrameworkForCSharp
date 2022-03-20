using System.Collections.Generic;
using TnyFramework.Net.DotNetty.Codec;
using TnyFramework.Net.DotNetty.Configuration.Guide;
namespace TnyFramework.Net.DotNetty.Configuration.Channel
{
    public interface IDataPacketV1ChannelMakerUnitContext
    {
        IList<IChannelPipelineChain> LoadPipelineChains(INetGuideUnitContext context);

        IDatagramPackDecoder LoadPackDecoder(INetGuideUnitContext context);

        IDatagramPackEncoder LoadPackEncoder(INetGuideUnitContext context);

        ICodecVerifier LoadCodecVerifier(INetGuideUnitContext context);

        ICodecCrypto LoadCodecCrypto(INetGuideUnitContext context);

        bool LoadCloseOnEncodeError();

        bool LoadCloseOnDecodeError();

        DataPacketV1Setting LoadEncodeConfig();

        DataPacketV1Setting LoadDecodeConfig();
    }
}
