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
using TnyFramework.Net.Hosting.Configuration;
using TnyFramework.Net.Hosting.Guide;
using TnyFramework.Net.Transport;

namespace TnyFramework.Net.Hosting.Rpc;

public interface IRpcHostConfiguration<TUnit, in TSetting, TContext, out TConfiguration, out TSpec> : INetHostConfiguration<TConfiguration>
    where TConfiguration : IRpcHostConfiguration<TUnit, TSetting, TContext, TConfiguration, TSpec>
    where TSetting : IServedServerSetting
    where TUnit : IServerGuide
    where TContext : INetGuideUnitContext
    where TSpec : INetGuideSpec<TUnit, TContext, TSpec>
{
    TConfiguration RpcServer(TSetting setting, Action<TSpec>? action = null);

    TConfiguration RpcAuthServiceSpecConfigure(Action<RpcAuthServiceSpec> action);

    TConfiguration IdGeneratorConfigure(Action<UnitSpec<IIdGenerator, IRpcUnitContext>> action);
}
