// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using Microsoft.Extensions.DependencyInjection;
using TnyFramework.DI.Units;
using TnyFramework.Net.Base;
using TnyFramework.Net.Command.Dispatcher.Monitor;
using TnyFramework.Net.DotNetty.Bootstrap;
using TnyFramework.Net.DotNetty.Codec;
using TnyFramework.Net.DotNetty.Configuration.Channel;
using TnyFramework.Net.DotNetty.Transport;
using TnyFramework.Net.Message;
using TnyFramework.Net.ProtobufNet;

namespace TnyFramework.Net.DotNetty.Configuration.Guide
{

    public abstract class NetGuideUnitContext : INetGuideUnitContext
    {
        internal IServiceCollection UnitContainer { get; }

        public INetUnitContext UnitContext { get; }

        public IDataPacketV1ChannelMakerUnitContext ChannelMakerUnitContext => ChannelMakerSpec;

        public UnitSpec<INettyTunnelFactory, INetGuideUnitContext> TunnelFactorySpec { get; }

        public UnitSpec<IMessageFactory, INetGuideUnitContext> MessageFactorySpec { get; }

        public UnitSpec<IContactFactory, INetGuideUnitContext> ContactFactorySpec { get; }

        public UnitSpec<IMessageBodyCodec, INetGuideUnitContext> MessageBodyCodecSpec { get; }

        public UnitSpec<IMessageHeaderCodec, INetGuideUnitContext> MessageHeaderCodecSpec { get; }

        public UnitSpec<RpcMonitor, INetGuideUnitContext> RpcMonitorSpec { get; }

        public UnitSpec<IMessageCodec, INetGuideUnitContext> MessageCodecSpec { get; }

        public DataPacketV1ChannelMakerSpec ChannelMakerSpec { get; }

        public abstract INetworkContext LoadNetworkContext();

        protected NetGuideUnitContext(INetUnitContext unitContext, IServiceCollection unitContainer)
        {
            UnitContainer = unitContainer;

            UnitContext = unitContext;

            // TunnelFactory
            TunnelFactorySpec = UnitSpec.Unit<INettyTunnelFactory, INetGuideUnitContext>();

            // MessageFactory
            MessageFactorySpec = UnitSpec.Unit<IMessageFactory, INetGuideUnitContext>()
                .Default<CommonMessageFactory>();

            // ContactFactory
            ContactFactorySpec = UnitSpec.Unit<IContactFactory, INetGuideUnitContext>()
                .Default<InnerContactFactory>();

            // MessageBodyCodec
            MessageBodyCodecSpec = UnitSpec.Unit<IMessageBodyCodec, INetGuideUnitContext>()
                .Default<TypeProtobufMessageBodyCodec>();

            // MessageBodyCodec
            MessageHeaderCodecSpec = UnitSpec.Unit<IMessageHeaderCodec, INetGuideUnitContext>()
                .Default<MessageHeaderCodec>();

            // MessageCodec
            MessageCodecSpec = UnitSpec.Unit<IMessageCodec, INetGuideUnitContext>()
                .Default(DefaultMessageCodec);

            // RpcMonitor
            RpcMonitorSpec = UnitSpec.Unit<RpcMonitor, INetGuideUnitContext>()
                .Default(DefaultRpcMonitor);

            // ChannelMaker 
            ChannelMakerSpec = new DataPacketV1ChannelMakerSpec(UnitContainer);

        }

        private RpcMonitor DefaultRpcMonitor(INetGuideUnitContext context)
        {
            return new RpcMonitor();
        }

        public void SetName(string name)
        {
            TunnelFactorySpec.WithNamePrefix(name);
            MessageFactorySpec.WithNamePrefix(name);
            ContactFactorySpec.WithNamePrefix(name);
            MessageBodyCodecSpec.WithNamePrefix(name);
            MessageHeaderCodecSpec.WithNamePrefix(name);
            MessageCodecSpec.WithNamePrefix(name);
            ChannelMakerSpec.SetName(name);
            OnSetName(name);
        }

        protected abstract void OnSetName(string name);

        public RpcMonitor LoadRpcMonitor()
        {
            return RpcMonitorSpec.Load(this, UnitContainer);
        }

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

        public IContactFactory LoadContactFactory()
        {
            return ContactFactorySpec.Load(this, UnitContainer);
        }

        public IMessageCodec LoadMessageCodec()
        {
            return MessageCodecSpec.Load(this, UnitContainer);
        }

        public IMessageBodyCodec LoadMessageBodyCodec()
        {
            return MessageBodyCodecSpec.Load(this, UnitContainer);
        }

        public IMessageHeaderCodec LoadMessageHeaderCodec()
        {
            return MessageHeaderCodecSpec.Load(this, UnitContainer);
        }

        private static IMessageCodec DefaultMessageCodec(INetGuideUnitContext context)
        {
            return new NettyMessageCodec(context.LoadMessageBodyCodec(), context.LoadMessageHeaderCodec());
        }
    }

}
