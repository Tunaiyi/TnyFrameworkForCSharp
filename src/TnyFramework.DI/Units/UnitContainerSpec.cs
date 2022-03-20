using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.Extensions.DependencyInjection;
using TnyFramework.Common.Extension;
using TnyFramework.DI.Container;
namespace TnyFramework.DI.Units
{
    public class UnitContainerSpec
    {
        public static UnitContainerSpec<TUnit, TSpec, TSpecImpl, TContext> Units<TUnit, TSpec, TSpecImpl, TContext>(Func<TSpecImpl> specFactory)
            where TSpec : IUnitSpec<TUnit, TContext>
            where TSpecImpl : UnitSpec<TUnit, TContext>, TSpec
        {
            return new UnitContainerSpec<TUnit, TSpec, TSpecImpl, TContext>(specFactory);
        }
    }

    public class UnitContainerSpec<TUnit, TSpec, TSpecImpl, TContext>
        : UnitContainerSpec, IUnitContainerSpec<TUnit, TSpec, TContext>
        where TSpec : IUnitSpec<TUnit, TContext>
        where TSpecImpl : UnitSpec<TUnit, TContext>, TSpec
    {
        /// <summary>
        /// 自定义设置列表
        /// </summary>
        private readonly List<TSpecImpl> specs = new List<TSpecImpl>();

        /// <summary>
        /// 默认设置列表
        /// </summary>
        private readonly List<TSpecImpl> defaultSpecs = new List<TSpecImpl>();

        /// <summary>
        /// 实例化 unit 列表
        /// </summary>
        private IList<TUnit> unitList;

        /// <summary>
        /// 实例化 unit Dictionary
        /// </summary>
        private IDictionary<string, TUnit> unitDictionary;

        /// <summary>
        /// 是否加载
        /// </summary>
        private bool loaded;

        /// <summary>
        /// 配置数量
        /// </summary>
        public int Size => specs.Count == 0 ? defaultSpecs.Count : specs.Count;

        /// <summary>
        /// 配置构造工厂
        /// </summary>
        private Func<TSpecImpl> specFactory;

        /// <summary>
        /// unit name 前缀
        /// </summary>
        private string namePrefix = string.Empty;


        public IUnitContainerSpec<TUnit, TSpec, TContext> WithNamePrefix(string prefix)
        {
            namePrefix = prefix;
            return this;
        }


        public UnitContainerSpec(Func<TSpecImpl> specFactory)
        {
            this.specFactory = specFactory;
        }


        public UnitContainerSpec<TUnit, TSpec, TSpecImpl, TContext> AddDefault(TUnit unit)
        {
            var spec = specFactory();
            spec.Default(unit);
            if (namePrefix.IsNotBlank())
            {
                spec.WithNamePrefix(namePrefix);
            }
            defaultSpecs.Add(spec);
            return this;
        }


        public IUnitContainerSpec<TUnit, TSpec, TContext> AddDefaultSpec(Action<TSpec> action)
        {
            var spec = specFactory();
            action.Invoke(spec);
            if (namePrefix.IsNotBlank())
            {
                spec.WithNamePrefix(namePrefix);
            }
            defaultSpecs.Add(spec);
            return this;
        }


        public UnitContainerSpec<TUnit, TSpec, TSpecImpl, TContext> AddDefault(string name, TUnit unit)
        {
            var spec = specFactory();
            spec.UnitName(name).Default(unit);
            if (namePrefix.IsNotBlank())
            {
                spec.WithNamePrefix(namePrefix);
            }
            defaultSpecs.Add(spec);
            return this;
        }


        public UnitContainerSpec<TUnit, TSpec, TSpecImpl, TContext> AddDefault(UnitCreator<TUnit, TContext> value)
        {
            var spec = specFactory();
            spec.Default(value);
            if (namePrefix.IsNotBlank())
            {
                spec.WithNamePrefix(namePrefix);
            }
            defaultSpecs.Add(spec);
            return this;
        }


        public UnitContainerSpec<TUnit, TSpec, TSpecImpl, TContext> AddDefault(string name, UnitCreator<TUnit, TContext> value)
        {
            var spec = specFactory();
            spec.UnitName(name).Default(value);
            defaultSpecs.Add(spec);
            return this;
        }



        public UnitContainerSpec<TUnit, TSpec, TSpecImpl, TContext> AddDefault<TImplement>() where TImplement : TUnit, new()
        {
            var spec = specFactory();
            spec.Default<TImplement>();
            if (namePrefix.IsNotBlank())
            {
                spec.WithNamePrefix(namePrefix);
            }
            defaultSpecs.Add(spec);
            return this;
        }


        public UnitContainerSpec<TUnit, TSpec, TSpecImpl, TContext> AddDefault<TImplement>(string name) where TImplement : TUnit, new()
        {
            var spec = specFactory();
            spec.UnitName(name).Default<TImplement>();
            defaultSpecs.Add(spec);
            return this;
        }


        public IUnitContainerSpec<TUnit, TSpec, TContext> Add(TUnit unit)
        {
            var spec = specFactory();
            spec.Unit(unit);
            if (namePrefix.IsNotBlank())
            {
                spec.WithNamePrefix(namePrefix);
            }
            specs.Add(spec);
            return this;
        }


        public IUnitContainerSpec<TUnit, TSpec, TContext> AddSpec(Action<TSpec> action)
        {
            var spec = specFactory();
            action.Invoke(spec);
            if (namePrefix.IsNotBlank())
            {
                spec.WithNamePrefix(namePrefix);
            }
            specs.Add(spec);
            return this;
        }



        public IUnitContainerSpec<TUnit, TSpec, TContext> AddSpec(string name, Action<TSpec> action)
        {
            var spec = specFactory();
            action.Invoke(spec);
            spec.UnitName(name);
            specs.Add(spec);
            return this;
        }


        public IUnitContainerSpec<TUnit, TSpec, TContext> Add(string name, TUnit unit)
        {
            var spec = specFactory();
            spec.UnitName(name).Unit(unit);
            specs.Add(spec);
            return this;
        }


        public IUnitContainerSpec<TUnit, TSpec, TContext> Add(UnitCreator<TUnit, TContext> value)
        {
            var spec = specFactory();
            spec.Creator(value);
            if (namePrefix.IsNotBlank())
            {
                spec.WithNamePrefix(namePrefix);
            }
            specs.Add(spec);
            return this;
        }


        public IUnitContainerSpec<TUnit, TSpec, TContext> Add(string name, UnitCreator<TUnit, TContext> value)
        {
            var spec = specFactory();
            spec.UnitName(name).Creator(value);
            specs.Add(spec);
            return this;
        }



        public IUnitContainerSpec<TUnit, TSpec, TContext> Add<TImplement>() where TImplement : TUnit, new()
        {
            var spec = specFactory();
            spec.Creator<TImplement>();
            if (namePrefix.IsNotBlank())
            {
                spec.WithNamePrefix(namePrefix);
            }
            specs.Add(spec);
            return this;
        }


        public IUnitContainerSpec<TUnit, TSpec, TContext> Add<TImplement>(string name) where TImplement : TUnit, new()
        {
            var spec = specFactory();
            spec.UnitName(name).Creator<TImplement>();
            specs.Add(spec);
            return this;
        }


        public IUnitContainerSpec<TUnit, TSpec, TContext> Clear()
        {
            specs.Clear();
            return this;
        }


        private void DoLoad(TContext context, IServiceCollection services)
        {
            if (loaded)
                return;
            var currentSpecs = specs.Count > 0 ? specs : defaultSpecs;
            var list = new List<TUnit>();
            var dictionary = new Dictionary<string, TUnit>();
            foreach (var currentSpec in currentSpecs)
            {
                var unit = currentSpec.Load(context, services);
                list.Add(unit);
                var name = currentSpec.GetUnitName();
                if (name.IsBlank())
                {
                    name = Unit.DefaultName(unit);
                }
                dictionary.Add(name, unit);
            }
            unitList = list.ToImmutableList();
            unitDictionary = dictionary.ToImmutableDictionary();
            loaded = true;
        }


        public IList<TUnit> Load(TContext context, IServiceCollection services)
        {
            DoLoad(context, services);
            return unitList;
        }


        public IDictionary<string, TUnit> LoadDictionary(TContext context, IServiceCollection services)
        {
            DoLoad(context, services);
            return unitDictionary;
        }
    }
}
