// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.Logging;
using TnyFramework.Common.Logger;

namespace TnyFramework.Common.Enum
{

    public abstract class BaseEnum
    {
        protected static readonly ConcurrentDictionary<Type, bool> TYPE_INIT_MAP = new ConcurrentDictionary<Type, bool>();
    }

    /// <summary>
    /// 枚举基类，所有枚举都必须继承此类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseEnum<T> : BaseEnum, IEnum where T : BaseEnum<T>, new()
    {
        private static readonly ILogger LOGGER = LogFactory.Logger<T>();
        private static readonly Dictionary<int, T> ID_ENUM_MAP = new Dictionary<int, T>();
        private static readonly Dictionary<string, T> NAME_ENUM_MAP = new Dictionary<string, T>();
        private static readonly List<T> ENUMS = new List<T>();

        private string name = "";
        private int id = int.MinValue;

        /// <summary>
        /// 枚举ID
        /// </summary>
        public int Id {
            get => id;
            private set {
                if (id == int.MinValue)
                {
                    id = value;
                }
            }
        }

        /// <summary>
        /// 枚举名称
        /// </summary>
        public string Name {
            get {
                CheckAndUpdateNames();
                return name;
            }
        }

        protected static TS E<TS>(int id, TS item, Action<TS>? builder = null) where TS : T
        {
            item.Id = id;
            builder?.Invoke(item);
            var type = typeof(T);
            lock (type)
            {
                if (ID_ENUM_MAP.TryGetValue(item.Id, out var exist))
                {
                    throw new ArgumentException($"{type} 枚举 {item.name} 与 {exist.name} 具有相同的 Id {item.Id}");
                }
                ID_ENUM_MAP.Add(item.Id, item);
                ENUMS.Add(item);
            }
            return item;
        }

        protected static T E(int id, Action<T>? builder = null)
        {
            var item = new T {
                Id = id,
            };
            builder?.Invoke(item);
            var type = typeof(T);
            lock (type)
            {
                if (ID_ENUM_MAP.TryGetValue(item.Id, out var exist))
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
            return GetById(id, false)!;
        }

        /// <summary>
        /// 通过名称获得枚举
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static T ForName(string name)
        {
            return GetByName(name)!;
        }

        /// <summary>
        /// 通过ID获得枚举
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static T? FindById(int id)
        {
            return GetById(id, false);
        }

        /// <summary>
        /// 通过名称获得枚举
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static T? FindByName(string name)
        {
            return GetByName(name, false);
        }

        /// <summary>
        /// 通过ID获得枚举
        /// </summary>
        /// <param name="id"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T GetById(int id, T defaultValue)
        {
            return GetById(id, false) ?? defaultValue;
        }

        /// <summary>
        /// 通过名称获得枚举
        /// </summary>
        /// <param name="name"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T GetByName(string name, T defaultValue)
        {
            return GetByName(name, false) ?? defaultValue;
        }

        /// <summary>
        /// 通过ID获得枚举
        /// </summary>
        /// <param name="id"></param>
        /// <param name="throwOnNull"></param>
        /// <returns></returns>
        protected static T? GetById(int id, bool throwOnNull = true)
        {
            CheckAndUpdateNames();
            if (ID_ENUM_MAP.TryGetValue(id, out var result))
                return result;
            if (throwOnNull)
            {
                throw new ArgumentException($"{typeof(T)} 枚举ID不存在 -> {id}");
            }
            return null;
        }

        /// <summary>
        /// 通过名称获得枚举
        /// </summary>
        /// <param name="name"></param>
        /// <param name="throwOnNull"></param>
        /// <returns></returns>
        protected static T? GetByName(string name, bool throwOnNull = true)
        {
            CheckAndUpdateNames();
            if (NAME_ENUM_MAP.TryGetValue(name, out var result))
                return result;
            if (throwOnNull)
            {
                throw new ArgumentException($"{typeof(T)} 枚举Name不存在 -> {name}");
            }
            return null;
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
        public static void LoadAll() => CheckAndUpdateNames();

        /// <summary>
        /// 获得所有枚举项
        /// </summary>
        /// <returns></returns>
        public static IReadOnlyCollection<T> GetValues<TEnum>()
        {
            LoadAll(typeof(TEnum));
            return ENUMS;
        }

        private static void CheckAndUpdateNames()
        {
            LoadAll(typeof(T));
        }

        protected virtual void OnCheck()
        {
        }

        /// <summary>
        /// 检查并更行名称
        /// </summary>
        protected static void LoadAll(Type type)
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
                var count = 0;
                var fields = type.GetFields(BindingFlags.Static | BindingFlags.Public);
                TYPE_INIT_MAP[type] = true;
                var result = true;
                try
                {
                    foreach (var field in fields)
                    {
                        object? item = null;
                        try
                        {
                            if (!type.IsAssignableFrom(field.FieldType) && !field.FieldType.IsAssignableFrom(type))
                            {
                                // LOGGER.LogWarning("Field {fieldType} {field} 不属于 Type {enumType}", field.FieldType, field.Name, type);
                                continue;
                            }
                            item = field.GetValue(type);
                            if (item == null)
                            {
                                result = false;
                                return;
                            }
                            if (!(item is T eItem))
                                continue;
                            eItem.name = field.Name;
                            if (NAME_ENUM_MAP.TryGetValue(eItem.Name, out var exist))
                            {
                                result = false;
                                throw new ArgumentException($"{type} 枚举 {eItem.name} 与 {exist.name} 具有相同的 Name {eItem.name}");
                            }
                            NAME_ENUM_MAP.Add(eItem.name, eItem);
                            count++;
                            eItem.OnCheck();
                        } catch (Exception e)
                        {
                            LOGGER.LogError(e, "Enum {Enum} Load {item} items", type, item);
                            Console.WriteLine(e);
                            throw;
                        }
                    }
                } finally
                {
                    TYPE_INIT_MAP[type] = result;
                    LOGGER.LogDebug("Enum {Enum} LoadAll {count} items", type, count);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object? obj)
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
            return $"{GetType().Name} {name}({id})";
        }

        public static implicit operator int(BaseEnum<T> type) => type.Id;

        public static explicit operator BaseEnum<T>(int type) => ForId(type);

        public static implicit operator T(BaseEnum<T> type)
        {
            if (type is T value)
                return value;
            return null!;
        }
    }

}
