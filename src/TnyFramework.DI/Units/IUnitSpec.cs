// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace TnyFramework.DI.Units
{

    public interface IUnitSpec<in TUnit, out TContext>
    {
        IUnitSpec<TUnit, TContext> Unit(TUnit value);

        IUnitSpec<TUnit, TContext> Creator<TImplement>() where TImplement : TUnit, new();

        IUnitSpec<TUnit, TContext> Creator(UnitCreator<TUnit, TContext> value);
    }

}
