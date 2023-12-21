// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using TnyFramework.Net.Application;
using TnyFramework.Net.Hosting.Guide;

namespace TnyFramework.Net.Hosting.Rpc
{

    public interface IRpcEndPointHostConfiguration<TUnit, in TSetting, TContext, out TConfiguration, out TSpec>
        : IRpcHostConfiguration<TUnit, TSetting, TContext, TConfiguration, TSpec>
        where TConfiguration : IRpcEndPointHostConfiguration<TUnit, TSetting, TContext, TConfiguration, TSpec>
        where TSetting : IServiceServerSetting
        where TUnit : INetServerGuide
        where TContext : INetGuideUnitContext
        where TSpec : INetGuideSpec<TUnit, TContext, TSpec>
    {
        TConfiguration RpcServer(string name, int port, Action<TSpec>? action = null);

        TConfiguration RpcServer(string name, string host, int port, Action<TSpec>? action = null);

        TConfiguration RpcServer(string name, string serveName, string host, int port, Action<TSpec>? action = null);
    }

}
