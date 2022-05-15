using System.Collections.Generic;
using System.Collections.Immutable;
using DotNetty.Transport.Channels;
using TnyFramework.Net.DotNetty.Bootstrap;

namespace TnyFramework.Net.DotNetty.Codec
{

    public abstract class ChannelMaker<TChannel> : AbstractChannelMaker<TChannel> where TChannel : IChannel
    {
        private IList<IChannelPipelineChain> channelPipelineChains = ImmutableList.Create<IChannelPipelineChain>();

        public override void InitChannel(TChannel channel)
        {
            var channelPipeline = channel.Pipeline;
            foreach (var chain in channelPipelineChains)
            {
                chain.BeforeMake(channelPipeline);
            }
            MakeChannel(channel);
            foreach (var chain in channelPipelineChains)
            {
                chain.AfterMake(channelPipeline);
            }
            PostInitChannel(channel);
        }

        public ChannelMaker<TChannel> SetChannelPipelineChains(IEnumerable<IChannelPipelineChain> chains)
        {
            channelPipelineChains = ImmutableList.CreateRange(chains);
            return this;
        }

        public ChannelMaker<TChannel> AddChannelPipelineChains(params IChannelPipelineChain[] chain)
        {
            var builder = ImmutableList.CreateBuilder<IChannelPipelineChain>();
            builder.AddRange(channelPipelineChains);
            builder.AddRange(chain);
            channelPipelineChains = builder.ToImmutableList();
            return this;
        }

        public ChannelMaker<TChannel> AddChannelPipelineChains(IEnumerable<IChannelPipelineChain> pipelineChains)
        {
            var builder = ImmutableList.CreateBuilder<IChannelPipelineChain>();
            builder.AddRange(channelPipelineChains);
            builder.AddRange(pipelineChains);
            channelPipelineChains = builder.ToImmutableList();
            return this;
        }

        public ChannelMaker<TChannel> AddChannelPipelineChains(IChannelPipelineChain chain)
        {
            var builder = ImmutableList.CreateBuilder<IChannelPipelineChain>();
            builder.AddRange(channelPipelineChains);
            builder.Add(chain);
            channelPipelineChains = builder.ToImmutableList();
            return this;
        }

        protected abstract void MakeChannel(TChannel channel);

        protected abstract void PostInitChannel(TChannel channel);
    }

}
