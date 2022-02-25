using System;
using TnyFramework.DI.Units;
using TnyFramework.Net.Base;
using TnyFramework.Net.Command;
using TnyFramework.Net.DotNetty.Bootstrap;
using TnyFramework.Net.DotNetty.Codec;
using TnyFramework.Net.DotNetty.Transport;
using TnyFramework.Net.Message;
namespace TnyFramework.Net.DotNetty.Configuration.Guide
{
    public interface INetGuideSpec<in TUnit, TUserId, out TContext, out TSpec> : IUnitSpec<TUnit, TContext>
        where TSpec : INetGuideSpec<TUnit, TUserId, TContext, TSpec>
        where TUnit : INetServer
        where TContext : INetGuideUnitContext
    {
        TSpec AnonymousId(TUserId anonymousUserId);

        TSpec CertificateConfigure(Action<IUnitSpec<ICertificateFactory<TUserId>, INetGuideUnitContext<TUserId>>> action);

        TSpec TunnelConfigure(Action<IUnitSpec<INettyTunnelFactory, INetGuideUnitContext<TUserId>>> action);

        TSpec MessageConfigure(Action<IUnitSpec<IMessageFactory, INetGuideUnitContext<TUserId>>> action);

        TSpec MessageBodyCodecConfigure(Action<UnitSpec<IMessageBodyCodec, INetGuideUnitContext<TUserId>>> action);

        TSpec MessageCodecConfigure(Action<UnitSpec<IMessageCodec, INetGuideUnitContext<TUserId>>> action);

        TSpec ChannelMakerConfigure(Action<UnitSpec<IChannelMaker, INetGuideUnitContext>> action);
    }
}
