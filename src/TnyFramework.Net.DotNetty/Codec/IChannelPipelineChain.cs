using DotNetty.Transport.Channels;

namespace TnyFramework.Net.DotNetty.Codec
{

    public interface IChannelPipelineChain
    {
        void BeforeMake(IChannelPipeline channelPipeline);

        void AfterMake(IChannelPipeline channelPipeline);
    }

}
