using System.Collections.Generic;
using TnyFramework.Net.DotNetty.Codec;
using TnyFramework.Net.DotNetty.Configuration.Guide;

namespace TnyFramework.Net.DotNetty.Configuration.Channel
{

    public interface IDataPacketV1ChannelMakerUnitContext
    {
        IList<IChannelPipelineChain> LoadPipelineChains(INetGuideUnitContext context);

        INetPacketDecoder LoadPackDecoder(INetGuideUnitContext context);

        INetPacketEncoder LoadPackEncoder(INetGuideUnitContext context);

        ICodecVerifier LoadCodecVerifier(INetGuideUnitContext context);

        ICodecCrypto LoadCodecCrypto(INetGuideUnitContext context);

        bool LoadCloseOnEncodeError();

        bool LoadCloseOnDecodeError();

        DataPacketV1Setting LoadEncodeConfig();

        DataPacketV1Setting LoadDecodeConfig();
    }

}
