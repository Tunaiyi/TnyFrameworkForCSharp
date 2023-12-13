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
using TnyFramework.Net.Command.Dispatcher.Monitor;
using TnyFramework.Net.DotNetty.Bootstrap;
using TnyFramework.Net.DotNetty.Codec;
using TnyFramework.Net.DotNetty.Transport;
using TnyFramework.Net.Message;

namespace TnyFramework.Net.DotNetty.Configuration.Guide
{

    public abstract class NetGuideSpec<TUnit, TContext, TUContext, TSpec>
        : UnitSpec<TUnit, TContext>, INetGuideSpec<TUnit, TContext, TSpec>
        where TUnit : INetServer
        where TContext : INetGuideUnitContext
        where TUContext : NetGuideUnitContext
        where TSpec : INetGuideSpec<TUnit, TContext, TSpec>
    {
        protected readonly TUContext context;

        protected NetGuideSpec(TUContext context)
        {
            this.context = context;
        }

        protected abstract TSpec Self();

        public TSpec TunnelConfigure(Action<IUnitSpec<INettyTunnelFactory, INetGuideUnitContext>> action)
        {
            action(context.TunnelFactorySpec);
            return Self();
        }

        public TSpec MessageConfigure(Action<IUnitSpec<IMessageFactory, INetGuideUnitContext>> action)
        {
            action(context.MessageFactorySpec);
            return Self();
        }

        public TSpec ContactConfigure(Action<IUnitSpec<IContactFactory, INetGuideUnitContext>> action)
        {
            action(context.ContactFactorySpec);
            return Self();
        }

        public TSpec MessageBodyCodecConfigure(Action<UnitSpec<IMessageBodyCodec, INetGuideUnitContext>> action)
        {
            action(context.MessageBodyCodecSpec);
            return Self();
        }

        public TSpec MessageHeaderCodecConfigure(Action<UnitSpec<IMessageHeaderCodec, INetGuideUnitContext>> action)
        {
            action(context.MessageHeaderCodecSpec);
            return Self();
        }

        public TSpec MessageCodecConfigure(Action<UnitSpec<IMessageCodec, INetGuideUnitContext>> action)
        {
            action(context.MessageCodecSpec);
            return Self();
        }

        public TSpec ChannelMakerConfigure(Action<UnitSpec<IChannelMaker, INetGuideUnitContext>> action)
        {
            action(context.ChannelMakerSpec);
            return Self();
        }

        public TSpec RpcMonitorConfigure(Action<UnitSpec<RpcMonitor, INetGuideUnitContext>> action)
        {
            action(context.RpcMonitorSpec);
            return Self();
        }
    }

}
