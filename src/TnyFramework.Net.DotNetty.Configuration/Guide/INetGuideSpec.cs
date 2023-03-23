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

    public interface INetGuideSpec<in TUnit, TUserId, out TContext, out TSpec> : IUnitSpec<TUnit, TContext>
        where TSpec : INetGuideSpec<TUnit, TUserId, TContext, TSpec>
        where TUnit : INetServer
        where TContext : INetGuideUnitContext
    {
        TSpec AnonymousId(TUserId anonymousUserId);

        TSpec CertificateConfigure(Action<IUnitSpec<ICertificateFactory<TUserId>, INetGuideUnitContext<TUserId>>> action);

        TSpec TunnelConfigure(Action<IUnitSpec<INettyTunnelFactory, INetGuideUnitContext<TUserId>>> action);

        TSpec MessageConfigure(Action<IUnitSpec<IMessageFactory, INetGuideUnitContext<TUserId>>> action);

        TSpec MessagerConfigure(Action<IUnitSpec<IMessagerFactory, INetGuideUnitContext<TUserId>>> action);

        TSpec MessageBodyCodecConfigure(Action<UnitSpec<IMessageBodyCodec, INetGuideUnitContext<TUserId>>> action);

        TSpec MessageCodecConfigure(Action<UnitSpec<IMessageCodec, INetGuideUnitContext<TUserId>>> action);

        TSpec ChannelMakerConfigure(Action<UnitSpec<IChannelMaker, INetGuideUnitContext>> action);

        TSpec RpcMonitorConfigure(Action<UnitSpec<RpcMonitor, INetGuideUnitContext<TUserId>>> action);
    }

}
