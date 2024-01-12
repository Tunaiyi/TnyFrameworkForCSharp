// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using Microsoft.Extensions.DependencyInjection;
using TnyFramework.DI.Units;
using TnyFramework.Net.DotNetty.Codec;
using TnyFramework.Net.DotNetty.Guide;
using TnyFramework.Net.DotNetty.Hosting.Channel;
using TnyFramework.Net.DotNetty.Transport;
using TnyFramework.Net.Hosting;
using TnyFramework.Net.Hosting.Guide;
using TnyFramework.Net.ProtobufNet;

namespace TnyFramework.Net.DotNetty.Hosting.Guide;

public abstract class NettyGuideUnitContext<TContext> : NetGuideUnitContext<TContext>, INettyGuideUnitContext
    where TContext : INettyGuideUnitContext
{
    public IDataPacketV1ChannelMakerUnitContext ChannelMakerUnitContext => ChannelMakerSpec;

    public UnitSpec<INettyTunnelFactory, TContext> TunnelFactorySpec { get; }

    public UnitSpec<IMessageBodyCodec, TContext> MessageBodyCodecSpec { get; }

    public UnitSpec<IMessageHeaderCodec, TContext> MessageHeaderCodecSpec { get; }

    public UnitSpec<IMessageCodec, TContext> MessageCodecSpec { get; }

    public DataPacketV1ChannelMakerSpec ChannelMakerSpec { get; }

    protected NettyGuideUnitContext(INetUnitContext unitContext, IServiceCollection unitContainer)
        : base(unitContext, unitContainer)
    {

        // TunnelFactory
        TunnelFactorySpec = UnitSpec.Unit<INettyTunnelFactory, TContext>();

        // MessageBodyCodec
        MessageBodyCodecSpec = UnitSpec.Unit<IMessageBodyCodec, TContext>()
            .Default<TypeProtobufMessageBodyCodec>();

        // MessageBodyCodec
        MessageHeaderCodecSpec = UnitSpec.Unit<IMessageHeaderCodec, TContext>()
            .Default<MessageHeaderCodec>();

        // MessageCodec
        MessageCodecSpec = UnitSpec.Unit<IMessageCodec, TContext>()
            .Default(DefaultMessageCodec);

        // ChannelMaker
        ChannelMakerSpec = new DataPacketV1ChannelMakerSpec(UnitContainer);

    }

    protected override void OnSetName(string name)
    {
        TunnelFactorySpec.WithNamePrefix(name);
        MessageBodyCodecSpec.WithNamePrefix(name);
        MessageHeaderCodecSpec.WithNamePrefix(name);
        MessageCodecSpec.WithNamePrefix(name);
        ChannelMakerSpec.SetName(name);
        NetworkContextSpec.WithNamePrefix(name);
        OnGuideUnitSetName(name);
    }

    protected abstract void OnGuideUnitSetName(string name);

    public IChannelMaker LoadChannelMaker()
    {
        return ChannelMakerSpec.Load(this, UnitContainer);
    }

    public INettyTunnelFactory LoadTunnelFactory()
    {
        return TunnelFactorySpec.Load(Self, UnitContainer);
    }

    public IMessageCodec LoadMessageCodec()
    {
        return MessageCodecSpec.Load(Self, UnitContainer);
    }

    public IMessageBodyCodec LoadMessageBodyCodec()
    {
        return MessageBodyCodecSpec.Load(Self, UnitContainer);
    }

    public IMessageHeaderCodec LoadMessageHeaderCodec()
    {
        return MessageHeaderCodecSpec.Load(Self, UnitContainer);
    }

    private static IMessageCodec DefaultMessageCodec(TContext context)
    {
        return new NettyMessageCodec(context.LoadMessageBodyCodec(), context.LoadMessageHeaderCodec());
    }
}
