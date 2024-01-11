// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using Microsoft.Extensions.DependencyInjection;
using TnyFramework.Net.Application;
using TnyFramework.Net.Hosting.Guide;

namespace TnyFramework.Net.Hosting.Rpc;

public abstract class RpcEndPointHostServerConfiguration<TConfiguration, TContext, TGuide, TSetting, TSpec>
    : RpcHostServerConfiguration<TConfiguration, TContext, TGuide, TSetting, TSpec>,
        IRpcEndPointHostConfiguration<TGuide, TSetting, TContext, TConfiguration, TSpec>
    where TSetting : IServedServerSetting
    where TGuide : IServerGuide<TSetting>
    where TContext : INetGuideUnitContext
    where TConfiguration : IRpcEndPointHostConfiguration<TGuide, TSetting, TContext, TConfiguration, TSpec>
    where TSpec : INetGuideSpec<TGuide, TContext, TSpec>
{
    protected RpcEndPointHostServerConfiguration(IServiceCollection unitContainer) : base(unitContainer)
    {
    }

    public abstract TConfiguration RpcServer(string name, int port, Action<TSpec>? action = null);

    public abstract TConfiguration RpcServer(string name, string host, int port, Action<TSpec>? action = null);

    public abstract TConfiguration RpcServer(string name, string serveName, string host, int port,
        Action<TSpec>? action = null);
}
