// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using TnyFramework.DI.Units;
using TnyFramework.Net.DotNetty.Bootstrap;
using TnyFramework.Net.DotNetty.Codec;
using TnyFramework.Net.DotNetty.Hosting.Guide;

namespace TnyFramework.Net.DotNetty.Hosting.Channel;

public class DataPacketV1ChannelMakerSpec : UnitSpec<IChannelMaker, INettyGuideUnitContext>,
    IDataPacketV1ChannelMakerSpec,
    IDataPacketV1ChannelMakerUnitContext
{
    private bool closeOnEncodeError = true;
    private bool closeOnDecodeError;

    private IServiceCollection UnitContainer { get; }

    private readonly UnitCollectionSpec<IChannelPipelineChain, INettyGuideUnitContext> channelPipelineChains;

    private readonly UnitSpec<ICodecVerifier, INettyGuideUnitContext> codecVerifier;

    private readonly UnitSpec<ICodecCrypto, INettyGuideUnitContext> codecCrypto;

    private readonly UnitSpec<INetPacketEncoder, INettyGuideUnitContext> packEncoder;

    private readonly UnitSpec<INetPacketDecoder, INettyGuideUnitContext> packDecoder;

    private readonly DataPacketV1EncodeSettingSpec encodeSettingSpec;

    private readonly DataPacketV1DecodeSettingSpec decodeSettingSpec;

    public DataPacketV1ChannelMakerSpec(IServiceCollection container, string unitName = "") : base(unitName)
    {
        UnitContainer = container;
        channelPipelineChains = UnitCollectionSpec.Units<IChannelPipelineChain, INettyGuideUnitContext>();
        codecVerifier = Unit<ICodecVerifier, INettyGuideUnitContext>().Default<NoopCodecVerifier>();
        codecCrypto = Unit<ICodecCrypto, INettyGuideUnitContext>().Default<NoopCodecCrypto>();
        packEncoder = Unit<INetPacketEncoder, INettyGuideUnitContext>().Default(CreatePackEncoder);
        packDecoder = Unit<INetPacketDecoder, INettyGuideUnitContext>().Default(CreatePackDecoder);
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

    private static IChannelMaker CreateChannelMaker(INettyGuideUnitContext context)
    {
        var makerContext = context.ChannelMakerUnitContext;
        var maker = new DatagramV1ChannelMaker(
            makerContext.LoadPackEncoder(context), makerContext.LoadCloseOnEncodeError(),
            makerContext.LoadPackDecoder(context), makerContext.LoadCloseOnDecodeError());
        var pipelineChains = makerContext.LoadPipelineChains(context);
        if (pipelineChains.Count > 0)
        {
            maker.AddChannelPipelineChains(pipelineChains);
        }
        return maker;
    }

    private static NetPacketV1Encoder CreatePackEncoder(INettyGuideUnitContext context)
    {
        var makerSpec = context.ChannelMakerUnitContext;
        return new NetPacketV1Encoder(makerSpec.LoadEncodeConfig(), context.LoadMessageCodec()) {
            CodecCrypto = makerSpec.LoadCodecCrypto(context),
            CodecVerifier = makerSpec.LoadCodecVerifier(context)
        };
    }

    private static NetPacketV1Decoder CreatePackDecoder(INettyGuideUnitContext context)
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

    public DataPacketV1ChannelMakerSpec PackEncoder(UnitCreator<INetPacketEncoder, INettyGuideUnitContext> factory)
    {
        packEncoder.Creator(factory);
        return this;
    }

    public DataPacketV1ChannelMakerSpec PackEncoder(Action<IUnitSpec<INetPacketEncoder, INettyGuideUnitContext>> action)
    {
        action.Invoke(packEncoder);
        return this;
    }

    public DataPacketV1ChannelMakerSpec PackDecoder(INetPacketDecoder decoder)
    {
        packDecoder.Unit(decoder);
        return this;
    }

    public DataPacketV1ChannelMakerSpec PackDecoder(UnitCreator<INetPacketDecoder, INettyGuideUnitContext> factory)
    {
        packDecoder.Creator(factory);
        return this;
    }

    public DataPacketV1ChannelMakerSpec PackDecoder(Action<IUnitSpec<INetPacketDecoder, INettyGuideUnitContext>> action)
    {
        action.Invoke(packDecoder);
        return this;
    }

    public DataPacketV1ChannelMakerSpec CodecVerifier(ICodecVerifier verifier)
    {
        codecVerifier.Unit(verifier);
        return this;
    }

    public DataPacketV1ChannelMakerSpec CodecVerifier(UnitCreator<ICodecVerifier, INettyGuideUnitContext> factory)
    {
        codecVerifier.Creator(factory);
        return this;
    }

    public DataPacketV1ChannelMakerSpec CodecVerifier(Action<IUnitSpec<ICodecVerifier, INettyGuideUnitContext>> action)
    {
        action.Invoke(codecVerifier);
        return this;
    }

    public DataPacketV1ChannelMakerSpec CodecCrypto(ICodecCrypto crypto)
    {
        codecCrypto.Unit(crypto);
        return this;
    }

    public DataPacketV1ChannelMakerSpec CodecCrypto(UnitCreator<ICodecCrypto, INettyGuideUnitContext> factory)
    {
        codecCrypto.Creator(factory);
        return this;
    }

    public DataPacketV1ChannelMakerSpec CodecCrypto(Action<IUnitSpec<ICodecCrypto, INettyGuideUnitContext>> action)
    {
        action.Invoke(codecCrypto);
        return this;
    }

    public DataPacketV1ChannelMakerSpec ChannelPipelineChains(Action<IUnitCollectionSpec<IChannelPipelineChain, INettyGuideUnitContext>> action)
    {
        action.Invoke(channelPipelineChains);
        return this;
    }

    public IList<IChannelPipelineChain> LoadPipelineChains(INettyGuideUnitContext context)
    {
        return channelPipelineChains.Load(context, UnitContainer);
    }

    public INetPacketDecoder LoadPackDecoder(INettyGuideUnitContext context)
    {
        return packDecoder.Load(context, UnitContainer);
    }

    public INetPacketEncoder LoadPackEncoder(INettyGuideUnitContext context)
    {
        return packEncoder.Load(context, UnitContainer);
    }

    public ICodecVerifier LoadCodecVerifier(INettyGuideUnitContext context)
    {
        return codecVerifier.Load(context, UnitContainer);
    }

    public ICodecCrypto LoadCodecCrypto(INettyGuideUnitContext context)
    {
        return codecCrypto.Load(context, UnitContainer);
    }

    public DataPacketV1Setting LoadEncodeConfig() => encodeSettingSpec.Setting;

    public DataPacketV1Setting LoadDecodeConfig() => decodeSettingSpec.Setting;

    public bool LoadCloseOnEncodeError() => closeOnEncodeError;

    public bool LoadCloseOnDecodeError() => closeOnDecodeError;
}
