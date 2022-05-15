using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using TnyFramework.DI.Units;
using TnyFramework.Net.DotNetty.Bootstrap;
using TnyFramework.Net.DotNetty.Codec;
using TnyFramework.Net.DotNetty.Configuration.Guide;

namespace TnyFramework.Net.DotNetty.Configuration.Channel
{

    public class DataPacketV1ChannelMakerSpec : UnitSpec<IChannelMaker, INetGuideUnitContext>, IDataPacketV1ChannelMakerSpec,
        IDataPacketV1ChannelMakerUnitContext
    {
        private bool closeOnEncodeError = true;
        private bool closeOnDecodeError;

        private IServiceCollection UnitContainer { get; }

        private readonly UnitCollectionSpec<IChannelPipelineChain, INetGuideUnitContext> channelPipelineChains;

        private readonly UnitSpec<ICodecVerifier, INetGuideUnitContext> codecVerifier;

        private readonly UnitSpec<ICodecCrypto, INetGuideUnitContext> codecCrypto;

        private readonly UnitSpec<INetPacketEncoder, INetGuideUnitContext> packEncoder;

        private readonly UnitSpec<INetPacketDecoder, INetGuideUnitContext> packDecoder;

        private readonly DataPacketV1EncodeSettingSpec encodeSettingSpec;

        private readonly DataPacketV1DecodeSettingSpec decodeSettingSpec;

        public DataPacketV1ChannelMakerSpec(IServiceCollection container, string unitName = "") : base(unitName)
        {
            UnitContainer = container;
            channelPipelineChains = UnitCollectionSpec.Units<IChannelPipelineChain, INetGuideUnitContext>();
            codecVerifier = Unit<ICodecVerifier, INetGuideUnitContext>().Default<NoopCodecVerifier>();
            codecCrypto = Unit<ICodecCrypto, INetGuideUnitContext>().Default<NoopCodecCrypto>();
            packEncoder = Unit<INetPacketEncoder, INetGuideUnitContext>().Default(CreatePackEncoder);
            packDecoder = Unit<INetPacketDecoder, INetGuideUnitContext>().Default(CreatePackDecoder);
            encodeSettingSpec = new DataPacketV1EncodeSettingSpec();
            decodeSettingSpec = new DataPacketV1DecodeSettingSpec();
            Default(CreateChannelMaker);
        }

        public void SetName(string name)
        {
            WithNamePrefix(name);
            channelPipelineChains.WithNamePrefix(name);
            codecVerifier.WithNamePrefix(name);
            codecCrypto.WithNamePrefix(name);
            packEncoder.WithNamePrefix(name);
            packDecoder.WithNamePrefix(name);
            encodeSettingSpec.WithNamePrefix(name);
            decodeSettingSpec.WithNamePrefix(name);
        }

        private static IChannelMaker CreateChannelMaker(INetGuideUnitContext context)
        {
            var makerContext = context.ChannelMakerUnitContext;
            var maker = new DatagramV1ChannelMaker(
                makerContext.LoadPackEncoder(context), makerContext.LoadCloseOnEncodeError(),
                makerContext.LoadPackDecoder(context), makerContext.LoadCloseOnDecodeError());
            var pipelineChains = makerContext.LoadPipelineChains(context);
            if (pipelineChains?.Count > 0)
            {
                maker.AddChannelPipelineChains(pipelineChains);
            }
            return maker;
        }

        private static NetPacketV1Encoder CreatePackEncoder(INetGuideUnitContext context)
        {
            var makerSpec = context.ChannelMakerUnitContext;
            return new NetPacketV1Encoder(makerSpec.LoadEncodeConfig(), context.LoadMessageCodec()) {
                CodecCrypto = makerSpec.LoadCodecCrypto(context),
                CodecVerifier = makerSpec.LoadCodecVerifier(context)
            };
        }

        private static NetPacketV1Decoder CreatePackDecoder(INetGuideUnitContext context)
        {
            var makerSpec = context.ChannelMakerUnitContext;
            return new NetPacketV1Decoder(makerSpec.LoadDecodeConfig(), context.LoadMessageCodec()) {
                CodecCrypto = makerSpec.LoadCodecCrypto(context),
                CodecVerifier = makerSpec.LoadCodecVerifier(context)
            };
        }

        public DataPacketV1ChannelMakerSpec CloseOnEncodeError(bool value)
        {
            closeOnEncodeError = value;
            return this;
        }

        public DataPacketV1ChannelMakerSpec CloseOnDecodeError(bool value)
        {
            closeOnDecodeError = value;
            return this;
        }

        public DataPacketV1ChannelMakerSpec PackEncodeSetting(Action<IDataPacketV1SettingSpec> action)
        {
            action.Invoke(encodeSettingSpec);
            return this;
        }

        public DataPacketV1ChannelMakerSpec PackDecodeSetting(Action<IDataPacketV1SettingSpec> action)
        {
            action.Invoke(decodeSettingSpec);
            return this;
        }

        public DataPacketV1ChannelMakerSpec PackEncoder(UnitCreator<INetPacketEncoder, INetGuideUnitContext> factory)
        {
            packEncoder.Creator(factory);
            return this;
        }

        public DataPacketV1ChannelMakerSpec PackEncoder(Action<IUnitSpec<INetPacketEncoder, INetGuideUnitContext>> action)
        {
            action.Invoke(packEncoder);
            return this;
        }

        public DataPacketV1ChannelMakerSpec PackDecoder(INetPacketDecoder decoder)
        {
            packDecoder.Unit(decoder);
            return this;
        }

        public DataPacketV1ChannelMakerSpec PackDecoder(UnitCreator<INetPacketDecoder, INetGuideUnitContext> factory)
        {
            packDecoder.Creator(factory);
            return this;
        }

        public DataPacketV1ChannelMakerSpec PackDecoder(Action<IUnitSpec<INetPacketDecoder, INetGuideUnitContext>> action)
        {
            action.Invoke(packDecoder);
            return this;
        }

        public DataPacketV1ChannelMakerSpec CodecVerifier(ICodecVerifier verifier)
        {
            codecVerifier.Unit(verifier);
            return this;
        }

        public DataPacketV1ChannelMakerSpec CodecVerifier(UnitCreator<ICodecVerifier, INetGuideUnitContext> factory)
        {
            codecVerifier.Creator(factory);
            return this;
        }

        public DataPacketV1ChannelMakerSpec CodecVerifier(Action<IUnitSpec<ICodecVerifier, INetGuideUnitContext>> action)
        {
            action.Invoke(codecVerifier);
            return this;
        }

        public DataPacketV1ChannelMakerSpec CodecCrypto(ICodecCrypto crypto)
        {
            codecCrypto.Unit(crypto);
            return this;
        }

        public DataPacketV1ChannelMakerSpec CodecCrypto(UnitCreator<ICodecCrypto, INetGuideUnitContext> factory)
        {
            codecCrypto.Creator(factory);
            return this;
        }

        public DataPacketV1ChannelMakerSpec CodecCrypto(Action<IUnitSpec<ICodecCrypto, INetGuideUnitContext>> action)
        {
            action.Invoke(codecCrypto);
            return this;
        }

        public DataPacketV1ChannelMakerSpec ChannelPipelineChains(Action<IUnitCollectionSpec<IChannelPipelineChain, INetGuideUnitContext>> action)
        {
            action.Invoke(channelPipelineChains);
            return this;
        }

        public IList<IChannelPipelineChain> LoadPipelineChains(INetGuideUnitContext context)
        {
            return channelPipelineChains.Load(context, UnitContainer);
        }

        public INetPacketDecoder LoadPackDecoder(INetGuideUnitContext context)
        {
            return packDecoder.Load(context, UnitContainer);
        }

        public INetPacketEncoder LoadPackEncoder(INetGuideUnitContext context)
        {
            return packEncoder.Load(context, UnitContainer);
        }

        public ICodecVerifier LoadCodecVerifier(INetGuideUnitContext context)
        {
            return codecVerifier.Load(context, UnitContainer);
        }

        public ICodecCrypto LoadCodecCrypto(INetGuideUnitContext context)
        {
            return codecCrypto.Load(context, UnitContainer);
        }

        public DataPacketV1Setting LoadEncodeConfig() => encodeSettingSpec.Setting;

        public DataPacketV1Setting LoadDecodeConfig() => decodeSettingSpec.Setting;

        public bool LoadCloseOnEncodeError() => closeOnEncodeError;

        public bool LoadCloseOnDecodeError() => closeOnDecodeError;
    }

}
