/**
 * 文件名称：GEnum.cs
 * 简   述：封装可以带参数的游戏枚举
 * 创建标识：riguang.l 2019/8/26
 **/

using System;
using System.Collections.Generic;
using System.Reflection;
namespace TnyFramework.Common.Enum
{
    /// <summary>
    /// 枚举
    /// </summary>
    public interface IEnum
    {
        /// <summary>
        /// 枚举ID
        /// </summary>
        int Id { get; }

        /// <summary>
        /// 枚举名称
        /// </summary>
        string Name { get; }
    }

    /// <summary>
    /// 枚举基类，所有枚举都必须继承此类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseEnum<T> : IEnum where T : BaseEnum<T>, new()
    {
        private const int STATUS_UNINITIALIZED = 0;
        private const int STATUS_INITIALIZING = 1;
        private const int STATUS_INITIALIZED = 2;

        private static volatile int _STATUS = STATUS_UNINITIALIZED;

        private static readonly Dictionary<int, T> ID_ENUM_MAP = new Dictionary<int, T>();
        private static readonly Dictionary<string, T> NAME_ENUM_MAP = new Dictionary<string, T>();
        private static readonly List<T> ENUMS = new List<T>();

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
        /// 检查并更行名称
        /// </summary>
        protected static void CheckAndUpdateNames()
        {
            var status = _STATUS;
            if (status != STATUS_UNINITIALIZED)
                return;
            var type = typeof(T);
            lock (type)
            {
                status = _STATUS;
                if (status != STATUS_UNINITIALIZED)
                    return;
                _STATUS = STATUS_INITIALIZING;
                var fields = type.GetFields(BindingFlags.Static | BindingFlags.Public);
                foreach (var field in fields)
                {
                    if (field.FieldType != type)
                        continue;
                    var item = (T)field.GetValue(type);
                    if (item == null)
                    {
                        _STATUS = STATUS_UNINITIALIZED;
                        return;
                    }
                    item.name = field.Name;
                    NAME_ENUM_MAP.TryGetValue(item.Name, out var exist);
                    if (exist != null)
                    {
                        throw new ArgumentException($"{type} 枚举 {item.name} 与 {exist.name} 具有相同的 Name {item.name}");
                    }
                    NAME_ENUM_MAP.Add(item.name, item);
                }
                _STATUS = STATUS_INITIALIZED;
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
