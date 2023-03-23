// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using TnyFramework.DI.Units;
using TnyFramework.Net.Base;
using TnyFramework.Net.Command;
using TnyFramework.Net.Command.Dispatcher.Monitor;
using TnyFramework.Net.DotNetty.Bootstrap;
using TnyFramework.Net.DotNetty.Codec;
using TnyFramework.Net.DotNetty.Transport;
using TnyFramework.Net.Message;

namespace TnyFramework.Net.DotNetty.Configuration.Guide
{

    public abstract class NetGuideSpec<TUnit, TUserId, TContext, TUContext, TSpec>
        : UnitSpec<TUnit, TContext>, INetGuideSpec<TUnit, TUserId, TContext, TSpec>
        where TUnit : INetServer
        where TContext : INetGuideUnitContext<TUserId>
        where TUContext : NetGuideUnitContext<TUserId>
        where TSpec : INetGuideSpec<TUnit, TUserId, TContext, TSpec>
    {
        protected readonly TUContext context;

        protected NetGuideSpec(TUContext context)
        {
            this.context = context;
        }

        protected abstract TSpec Self();

        public TSpec TunnelConfigure(Action<IUnitSpec<INettyTunnelFactory, INetGuideUnitContext<TUserId>>> action)
        {
            action(context.TunnelFactorySpec);
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
            action(context.CertificateFactorySpec);
            return Self();
        }

        public TSpec MessageConfigure(Action<IUnitSpec<IMessageFactory, INetGuideUnitContext<TUserId>>> action)
        {
            action(context.MessageFactorySpec);
            return Self();
        }

        public TSpec MessagerConfigure(Action<IUnitSpec<IMessagerFactory, INetGuideUnitContext<TUserId>>> action)
        {
            action(context.MessagerFactorySpec);
            return Self();
        }

        public TSpec MessageBodyCodecConfigure(Action<UnitSpec<IMessageBodyCodec, INetGuideUnitContext<TUserId>>> action)
        {
            action(context.MessageBodyCodecSpec);
            return Self();
        }

        public TSpec MessageHeaderCodecConfigure(Action<UnitSpec<IMessageHeaderCodec, INetGuideUnitContext<TUserId>>> action)
        {
            action(context.MessageHeaderCodecSpec);
            return Self();
        }

        public TSpec MessageCodecConfigure(Action<UnitSpec<IMessageCodec, INetGuideUnitContext<TUserId>>> action)
        {
            action(context.MessageCodecSpec);
            return Self();
        }

        public TSpec ChannelMakerConfigure(Action<UnitSpec<IChannelMaker, INetGuideUnitContext>> action)
        {
            action(context.ChannelMakerSpec);
            return Self();
        }

        public TSpec RpcMonitorConfigure(Action<UnitSpec<RpcMonitor, INetGuideUnitContext<TUserId>>> action)
        {
            action(context.RpcMonitorSpec);
            return Self();
        }
    }

}
