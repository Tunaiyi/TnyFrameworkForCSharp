using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace TnyFramework.Common.Enum
{

    /// <summary>
    /// 枚举基类，所有枚举都必须继承此类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseEnum<T> : IEnum where T : BaseEnum<T>, new()
    {
        private static readonly ConcurrentDictionary<Type, bool> TYPE_INIT_MAP = new ConcurrentDictionary<Type, bool>();
        protected static readonly Dictionary<int, T> ID_ENUM_MAP = new Dictionary<int, T>();
        protected static readonly Dictionary<string, T> NAME_ENUM_MAP = new Dictionary<string, T>();
        protected static readonly List<T> ENUMS = new List<T>();

        private string name;

        /// <summary>
        /// 枚举ID
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// 枚举名称
        /// </summary>
        public string Name {
            get {
                CheckAndUpdateNames();
                return name;
            }
        }


        protected static T E(int id, T item, Action<T> builder = null)
        {
            item.Id = id;
            builder?.Invoke(item);
            var type = typeof(T);
            lock (type)
            {
                ID_ENUM_MAP.TryGetValue(item.Id, out var exist);
                if (exist != null)
                {
                    throw new ArgumentException($"{type} 枚举 {item.name} 与 {exist.name} 具有相同的 Id {item.Id}");
                }
                ID_ENUM_MAP.Add(item.Id, item);
                ENUMS.Add(item);
            }
            return item;
        }


        protected static T E(int id, Action<T> builder = null)
        {
            var item = new T {
                Id = id,
            };
            builder?.Invoke(item);
            var type = typeof(T);
            lock (type)
            {
                ID_ENUM_MAP.TryGetValue(item.Id, out var exist);
                if (exist != null)
                {
                    throw new ArgumentException($"{type} 枚举 {item.name} 与 {exist.name} 具有相同的 Id {item.Id}");
                }
                ID_ENUM_MAP.Add(item.Id, item);
                ENUMS.Add(item);
            }
            return item;
        }


        /// <summary>
        /// 通过ID获得枚举
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static T ForId(int id)
        {
            CheckAndUpdateNames();
            if (!ID_ENUM_MAP.TryGetValue(id, out var result))
            {
                throw new ArgumentException("枚举ID不存在 -> " + id);
            }
            return result;
        }


        /// <summary>
        /// 通过名称获得枚举
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static T ForName(string name)
        {
            CheckAndUpdateNames();
            if (!NAME_ENUM_MAP.TryGetValue(name, out var result))
            {
                throw new ArgumentException("枚举名称不存在 -> " + name);
            }
            return result;
        }


        /// <summary>
        /// 获得所有枚举项
        /// </summary>
        /// <returns></returns>
        public static IReadOnlyCollection<T> GetValues()
        {
            CheckAndUpdateNames();
            return ENUMS;
        }


        /// <summary>
        /// 获得所有枚举项
        /// </summary>
        /// <returns></returns>
        public static IReadOnlyCollection<T> GetValues<TEnum>()
        {
            CheckAndUpdateNames(typeof(TEnum));
            return ENUMS;
        }


        private static void CheckAndUpdateNames()
        {
            CheckAndUpdateNames(typeof(T));
        }


        /// <summary>
        /// 检查并更行名称
        /// </summary>
        protected static void CheckAndUpdateNames(Type type)
        {
            if (TYPE_INIT_MAP.ContainsKey(type))
            {
                return;
            }
            var currentType = typeof(T);
            lock (currentType)
            {
                if (TYPE_INIT_MAP.ContainsKey(type))
                {
                    return;
                }
                var fields = type.GetFields(BindingFlags.Static | BindingFlags.Public);
                TYPE_INIT_MAP[type] = true;
                var result = true;
                try
                {
                    foreach (var field in fields)
                    {
                        if (!field.FieldType.IsAssignableFrom(type))
                            continue;
                        var item = field.GetValue(type);
                        if (item == null)
                        {
                            result = false;
                            return;
                        }
                        if (!(item is T eItem))
                            continue;
                        eItem.name = field.Name;
                        NAME_ENUM_MAP.TryGetValue(eItem.Name, out var exist);
                        if (exist != null)
                        {
                            result = false;
                            throw new ArgumentException($"{type} 枚举 {eItem.name} 与 {exist.name} 具有相同的 Name {eItem.name}");
                        }
                        NAME_ENUM_MAP.Add(eItem.name, eItem);
                    }
                } finally
                {
                    TYPE_INIT_MAP[type] = result;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (!(obj is T baseEnum))
                return false;
            return baseEnum.Id == Id;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }


        /// <summary>
        /// 等价于Name
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name;
        }
    }

}