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
using TnyFramework.Net.Message;

namespace TnyFramework.Net.Configuration.Guide
{

    public interface INetGuideSpec<TUnit, TContext, out TSpec>
        : IUnitSpec<TUnit, TContext>, INetServerGuideBuilder<TUnit>
        where TSpec : INetGuideSpec<TUnit, TContext, TSpec>
        where TUnit : INetServerGuide
        where TContext : INetGuideUnitContext
    {
        TSpec MessageConfigure(Action<IUnitSpec<IMessageFactory, TContext>> action);

        TSpec ContactConfigure(Action<IUnitSpec<IContactFactory, TContext>> action);

        TSpec RpcMonitorConfigure(Action<UnitSpec<RpcMonitor, TContext>> action);
    }

}
