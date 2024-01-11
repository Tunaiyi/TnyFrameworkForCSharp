// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using TnyFramework.DI.Units;
using TnyFramework.Net.Application;

namespace TnyFramework.Net.Hosting.App;

public class NetAppContextSpec : UnitSpec<NetAppContext, INetUnitContext>, INetAppContextSpec
{
    private readonly NetAppContext context = new();

    public NetAppContextSpec(string unitName = "") : base(unitName)
    {
        Default(_ => context);
    }

    public NetAppContextSpec ServerId(int value)
    {
        context.ServerId = value;
        return this;

    }

    public NetAppContextSpec AppName(string value)
    {
        context.Name = value;
        return this;
    }

    public NetAppContextSpec AppType(string value)
    {
        context.AppType = value;
        return this;
    }

    public NetAppContextSpec ScopeType(string value)
    {
        context.ScopeType = value;
        return this;
    }

    public NetAppContextSpec Locale(string value)
    {
        context.Locale = value;
        return this;
    }
}
