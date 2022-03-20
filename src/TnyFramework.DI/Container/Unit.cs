using System;
using System.Collections.Concurrent;
namespace TnyFramework.DI.Container
{
    public abstract class Unit
    {
        private static readonly ConcurrentDictionary<Type, Type> UNIT_TYPES = new ConcurrentDictionary<Type, Type>();
        private static readonly ConcurrentDictionary<Type, Type> I_UNIT_TYPES = new ConcurrentDictionary<Type, Type>();


        internal static Type UnitType(Type type)
        {
            return I_UNIT_TYPES.GetOrAdd(type, key => typeof(IUnit<>).MakeGenericType(key));
        }


        private static Type GetUnitType(Type type)
        {
            return UNIT_TYPES.GetOrAdd(type, key => typeof(Unit<>).MakeGenericType(key));
        }


        internal static Unit Create(Type type, IServiceInstance instance, string name = "")
        {
            var unitType = GetUnitType(type);
            var unit = (Unit)Activator.CreateInstance(unitType);
            unit.Name = name;
            unit.Instance = instance;
            return unit;
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
            return type.FullName;
        }


        public static string DefaultName(object type)
        {
            return type.GetType().FullName;
        }


        public static string DefaultName(Type type)
        {
            return type.FullName;
        }


        protected Unit()
        {
        }


        protected Unit(string name, IServiceInstance instance)
        {
            Name = name;
            Instance = instance;
        }


        public string Name { get; private set; }

        public IServiceInstance Instance { get; private set; }
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
            return (TUnit)Instance.Get(provider);
        }
    }
}
