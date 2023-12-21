// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Collections.Concurrent;
using TnyFramework.DI.Units;

namespace TnyFramework.DI.Container
{

    public abstract class Unit
    {
        private static readonly ConcurrentDictionary<Type, Type> UNIT_TYPES = new();
        private static readonly ConcurrentDictionary<Type, Type> UNIT_INTERFACE_TYPES = new();

        internal static Type UnitType(Type type)
        {
            return UNIT_INTERFACE_TYPES.GetOrAdd(type, key => typeof(IUnit<>).MakeGenericType(key));
        }

        private static Type GetUnitType(Type type)
        {
            return UNIT_TYPES.GetOrAdd(type, key => typeof(Unit<>).MakeGenericType(key));
        }

        internal static Unit Create(Type type, IServiceInstance instance, string name = "")
        {
            var unitType = GetUnitType(type);
            var unitObject = Activator.CreateInstance(unitType);
            if (unitObject is Unit unit)
            {
                unit.Name = name;
                unit.Instance = instance;
                return unit;
            }
            throw new ArgumentException($"{unitType} is not {nameof(Unit)}");
        }

        internal static Unit<TUnit> Create<TUnit>(IServiceInstance instance, string name = "")
        {
            return new Unit<TUnit>(name, instance);
        }

        public static string UnitName<T>(string prefix)
        {
            var type = typeof(T);
            return prefix + type.Name;
        }

        public static string UnitName(object type, string prefix)
        {
            return prefix + type.GetType().Name;
        }

        public static string UnitName(Type type, string prefix)
        {
            return prefix + type.Name;
        }

        public static string DefaultName<T>()
        {
            var type = typeof(T);
            return type.FullName!;
        }

        public static string DefaultName(object type)
        {
            return type.GetType().FullName!;
        }

        public static string DefaultName(Type type)
        {
            return type.FullName!;
        }

        protected Unit()
        {
        }

        protected Unit(string name, IServiceInstance instance)
        {
            Name = name;
            Instance = instance;
        }

        public string Name { get; private set; } = null!;

        public IServiceInstance Instance { get; private set; } = null!;
    }

    public class Unit<TUnit> : Unit, IUnit<TUnit>
    {
        public Unit()
        {
        }

        public Unit(string name, IServiceInstance instance) : base(name, instance)
        {
        }

        public TUnit Value(IServiceProvider provider)
        {
            return (TUnit) Instance.Get(provider);
        }
    }

}
