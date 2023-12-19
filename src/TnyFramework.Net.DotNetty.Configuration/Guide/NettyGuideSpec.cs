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
using TnyFramework.Net.Configuration.Guide;
using TnyFramework.Net.DotNetty.Bootstrap;
using TnyFramework.Net.DotNetty.Codec;
using TnyFramework.Net.DotNetty.Transport;

namespace TnyFramework.Net.DotNetty.Configuration.Guide
{

    public abstract class NettyGuideSpec<TUnit, TContext, TImplContext, TSpec>
        : NetGuideSpec<TUnit, TContext, TImplContext, TSpec>, INettyGuideSpec<TUnit, TContext, TSpec>
        where TUnit : INetServerGuide
        where TContext : INettyGuideUnitContext
        where TImplContext : NettyGuideUnitContext<TContext>
        where TSpec : INettyGuideSpec<TUnit, TContext, TSpec>
    {
        protected NettyGuideSpec(TImplContext context) : base(context)
        {
        }

        protected abstract override TSpec Self();

        public TSpec TunnelConfigure(Action<IUnitSpec<INettyTunnelFactory, TContext>> action)
        {
            action(context.TunnelFactorySpec);
            return Self();
        }

        public TSpec MessageBodyCodecConfigure(Action<UnitSpec<IMessageBodyCodec, TContext>> action)
        {
            action(context.MessageBodyCodecSpec);
            return Self();
        }

        public TSpec MessageHeaderCodecConfigure(Action<UnitSpec<IMessageHeaderCodec, TContext>> action)
        {
            action(context.MessageHeaderCodecSpec);
            return Self();
        }

        public TSpec MessageCodecConfigure(Action<UnitSpec<IMessageCodec, TContext>> action)
        {
            action(context.MessageCodecSpec);
            return Self();
        }

        public TSpec ChannelMakerConfigure(Action<UnitSpec<IChannelMaker, INettyGuideUnitContext>> action)
        {
            action(context.ChannelMakerSpec);
            return Self();
        }
    }

}
