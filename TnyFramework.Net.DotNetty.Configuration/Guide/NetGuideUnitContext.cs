using Microsoft.Extensions.DependencyInjection;
using TnyFramework.DI.Units;
using TnyFramework.Net.Base;
using TnyFramework.Net.Command;
using TnyFramework.Net.DotNetty.Bootstrap;
using TnyFramework.Net.DotNetty.Codec;
using TnyFramework.Net.DotNetty.Configuration.Channel;
using TnyFramework.Net.DotNetty.Transport;
using TnyFramework.Net.Message;
using TnyFramework.Net.TypeProtobuf;
namespace TnyFramework.Net.DotNetty.Configuration.Guide
{
    public abstract class NetGuideUnitContext<TUserId> : INetGuideUnitContext<TUserId>
    {
        internal IServiceCollection UnitContainer { get; }

        public INetUnitContext UnitContext { get; }

        public IDataPacketV1ChannelMakerUnitContext ChannelMakerUnitContext => ChannelMakerSpec;

        public UnitSpec<INettyTunnelFactory, INetGuideUnitContext<TUserId>> TunnelFactorySpec { get; }

        public UnitSpec<IMessageFactory, INetGuideUnitContext<TUserId>> MessageFactorySpec { get; }

        public UnitSpec<IMessageBodyCodec, INetGuideUnitContext<TUserId>> MessageBodyCodecSpec { get; }

        public UnitSpec<IMessageCodec, INetGuideUnitContext<TUserId>> MessageCodecSpec { get; }

        public UnitSpec<ICertificateFactory<TUserId>, INetGuideUnitContext<TUserId>> CertificateFactorySpec { get; }

        public DataPacketV1ChannelMakerSpec ChannelMakerSpec { get; }

        public abstract INetworkContext LoadNetworkContext();


        protected NetGuideUnitContext(INetUnitContext unitContext, IServiceCollection unitContainer)
        {
            UnitContainer = unitContainer;

            UnitContext = unitContext;

            // TunnelFactory
            TunnelFactorySpec = UnitSpec.Unit<INettyTunnelFactory, INetGuideUnitContext<TUserId>>();

            // MessageFactory
            MessageFactorySpec = UnitSpec.Unit<IMessageFactory, INetGuideUnitContext<TUserId>>()
                .Default<CommonMessageFactory>();


            // MessageBodyCodec
            MessageBodyCodecSpec = UnitSpec.Unit<IMessageBodyCodec, INetGuideUnitContext<TUserId>>()
                .Default<TypeProtobufMessageBodyCodec>();

            // MessageCodec
            MessageCodecSpec = UnitSpec.Unit<IMessageCodec, INetGuideUnitContext<TUserId>>()
                .Default(DefaultMessageCodec);


            // CertificateFactory
            CertificateFactorySpec = UnitSpec.Unit<ICertificateFactory<TUserId>, INetGuideUnitContext<TUserId>>()
                .Default<CertificateFactory<TUserId>>();

            // ChannelMaker 
            ChannelMakerSpec = new DataPacketV1ChannelMakerSpec(UnitContainer);

        }


        public void SetName(string name)
        {
            TunnelFactorySpec.WithNamePrefix(name);
            MessageFactorySpec.WithNamePrefix(name);
            MessageBodyCodecSpec.WithNamePrefix(name);
            MessageCodecSpec.WithNamePrefix(name);
            CertificateFactorySpec.WithNamePrefix(name);
            ChannelMakerSpec.SetName(name);
            OnSetName(name);
        }


        protected abstract void OnSetName(string name);


        public IChannelMaker LoadChannelMaker()
        {
            return ChannelMakerSpec.Load(this, UnitContainer);
        }


        public INettyTunnelFactory LoadTunnelFactory()
        {
            return TunnelFactorySpec.Load(this, UnitContainer);
        }



        public IMessageFactory LoadMessageFactory()
        {
            return MessageFactorySpec.Load(this, UnitContainer);
        }



        public IMessageCodec LoadMessageCodec()
        {
            return MessageCodecSpec.Load(this, UnitContainer);
        }


        public IMessageBodyCodec LoadMessageBodyCodec()
        {
            return MessageBodyCodecSpec.Load(this, UnitContainer);
        }


        public ICertificateFactory<TUserId> LoadCertificateFactory()
        {
            return CertificateFactorySpec.Load(this, UnitContainer);
        }


        private static IMessageCodec DefaultMessageCodec(INetGuideUnitContext<TUserId> context)
        {
            return new NettyMessageCodec(context.LoadMessageBodyCodec());
        }
    }
}
