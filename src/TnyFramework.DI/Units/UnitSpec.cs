// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using Microsoft.Extensions.DependencyInjection;
using TnyFramework.Common.Extensions;
using TnyFramework.DI.Container;

namespace TnyFramework.DI.Units
{

    public class UnitSpec
    {
        public static UnitSpec<TUnit, TContext> Unit<TUnit, TContext>()
        {
            return new UnitSpec<TUnit, TContext>();
        }
    }

    public class UnitSpec<TUnit, TContext> : UnitSpec, IUnitSpec<TUnit, TContext>
    {
        private string unitName;

        private UnitCreator<TUnit, TContext> defaultCreator;

        private UnitCreator<TUnit, TContext> creator;

        private TUnit unit;

        private bool created;

        public string GetUnitName()
        {
            return unitName;
        }

        public UnitSpec(string unitName = "")
        {
            this.unitName = unitName;
        }

        public UnitSpec<TUnit, TContext> Default(TUnit value)
        {
            if (defaultCreator != null)
                return this;
            defaultCreator = _ => value;
            return this;
        }

        public UnitSpec<TUnit, TContext> Default(UnitCreator<TUnit, TContext> value)
        {
            if (defaultCreator != null)
                return this;
            defaultCreator = value;
            return this;
        }

        public UnitSpec<TUnit, TContext> UnitName(string value)
        {
            if (unitName.IsBlank())
                unitName = value;
            return this;
        }

        public UnitSpec<TUnit, TContext> WithNamePrefix(string prefix)
        {
            if (unitName.IsBlank())
                unitName = prefix + typeof(TUnit).Name;
            return this;
        }

        public UnitSpec<TUnit, TContext> WithNamePrefix<TType>(string prefix)
        {
            if (unitName.IsBlank())
                unitName = prefix + typeof(TType).Name;
            return this;
        }

        public UnitSpec<TUnit, TContext> Default<TImplement>()
            where TImplement : TUnit, new()
        {
            if (defaultCreator != null)
                return this;
            defaultCreator = _ => new TImplement();
            return this;
        }

        public IUnitSpec<TUnit, TContext> Unit(TUnit value)
        {
            if (unit != null)
                return this;
            unit = value;
            return this;
        }

        public IUnitSpec<TUnit, TContext> Creator<TImplement>()
            where TImplement : TUnit, new()
        {
            if (creator != null)
                return this;
            creator = _ => new TImplement();
            return this;
        }

        public IUnitSpec<TUnit, TContext> Creator(UnitCreator<TUnit, TContext> value)
        {
            if (creator != null)
                return this;
            creator = value;
            return this;
        }

        public TUnit Load(TContext context, IServiceCollection services)
        {
            try
            {
                if (unit != null)
                {
                    return unit;
                }
                if (creator != null)
                {
                    return unit = creator.Invoke(context);
                }
                if (defaultCreator != null)
                {
                    return unit = defaultCreator.Invoke(context);
                }
            } finally
            {
                if (!created && unit != null)
                {
                    services?.AddSingletonUnit(unitName, unit);
                    created = true;
                }
            }
            throw new NullReferenceException($"{nameof(TUnit)} is null");
        }
    }

}
