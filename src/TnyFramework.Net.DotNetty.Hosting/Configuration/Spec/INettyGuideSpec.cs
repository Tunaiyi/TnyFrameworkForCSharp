// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using TnyFramework.DI.Units;
using TnyFramework.Net.Application;
using TnyFramework.Net.DotNetty.Bootstrap;
using TnyFramework.Net.DotNetty.Codec;
using TnyFramework.Net.DotNetty.Hosting.Guide;
using TnyFramework.Net.DotNetty.Transport;
using TnyFramework.Net.Hosting.Guide;

namespace TnyFramework.Net.DotNetty.Hosting.Configuration.Spec;

public interface INettyGuideSpec<TGuide, TContext, out TSpec> : INetGuideSpec<TGuide, TContext, TSpec>
    where TGuide : IServerGuide<IServiceServerSetting>
    where TContext : INettyGuideUnitContext
    where TSpec : INetGuideSpec<TGuide, TContext, TSpec>
{
    public TSpec TunnelConfigure(Action<IUnitSpec<INettyTunnelFactory, TContext>> action);

    public TSpec MessageBodyCodecConfigure(Action<UnitSpec<IMessageBodyCodec, TContext>> action);

    public TSpec MessageHeaderCodecConfigure(Action<UnitSpec<IMessageHeaderCodec, TContext>> action);

    public TSpec MessageCodecConfigure(Action<UnitSpec<IMessageCodec, TContext>> action);

    public TSpec ChannelMakerConfigure(Action<UnitSpec<IChannelMaker, INettyGuideUnitContext>> action);
}
