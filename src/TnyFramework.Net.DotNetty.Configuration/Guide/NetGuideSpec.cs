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
    public abstract class NetGuideSpec<TUnit, TUserId, TContext, UContext, TSpec>
        : UnitSpec<TUnit, TContext>, INetGuideSpec<TUnit, TUserId, TContext, TSpec>
        where TUnit : INetServer
        where TContext : INetGuideUnitContext<TUserId>
        where UContext : NetGuideUnitContext<TUserId>
        where TSpec : INetGuideSpec<TUnit, TUserId, TContext, TSpec>
    {
        protected readonly UContext context;


        protected NetGuideSpec(UContext context)
        {
            this.context = context;
        }


        protected abstract TSpec Self();


        public TSpec TunnelConfigure(Action<IUnitSpec<INettyTunnelFactory, INetGuideUnitContext<TUserId>>> action)
        {
            action.Invoke(context.TunnelFactorySpec);
            return Self();
        }


        public TSpec AnonymousId(TUserId anonymousUserId)
        {

            context.CertificateFactorySpec.Creator(c => new CertificateFactory<TUserId>(anonymousUserId));
            return Self();
        }


        public TSpec CertificateConfigure(
            Action<IUnitSpec<ICertificateFactory<TUserId>, INetGuideUnitContext<TUserId>>> action)
        {
            action.Invoke(context.CertificateFactorySpec);
            return Self();
        }


        public TSpec MessageConfigure(Action<IUnitSpec<IMessageFactory, INetGuideUnitContext<TUserId>>> action)
        {
            action.Invoke(context.MessageFactorySpec);
            return Self();
        }


        public TSpec MessageBodyCodecConfigure(Action<UnitSpec<IMessageBodyCodec, INetGuideUnitContext<TUserId>>> action)
        {
            action.Invoke(context.MessageBodyCodecSpec);
            return Self();
        }


        public TSpec MessageCodecConfigure(Action<UnitSpec<IMessageCodec, INetGuideUnitContext<TUserId>>> action)
        {
            action.Invoke(context.MessageCodecSpec);
            return Self();
        }


        public TSpec ChannelMakerConfigure(Action<UnitSpec<IChannelMaker, INetGuideUnitContext>> action)
        {
            action.Invoke(context.ChannelMakerSpec);
            return Self();
        }
    }
}
