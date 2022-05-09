using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using TnyFramework.Common.Enum;

namespace TnyFramework.Codec
{

    /// <summary>
    /// 媒体类型
    /// </summary>
    public interface IMimeType : IEnum
    {
        string MetaType { get; }
    }

    public class MimeType : BaseEnum<MimeType>, IMimeType
    {
        private static readonly ConcurrentDictionary<string, MimeType> META_TYPE_MAP = new ConcurrentDictionary<string, MimeType>();

        public string MetaType { get; protected set; }


        protected override void OnCheck()
        {
            if (META_TYPE_MAP.TryAdd(MetaType, this))
                return;
            var value = META_TYPE_MAP[MetaType];
            if (!ReferenceEquals(value, this))
            {
                throw new ArgumentException($"{value} 与 {this} 存在相同的 Type {MetaType}");
            }
        }


        public new static MimeType ForId(int id)
        {
            return BaseEnum<MimeType>.ForId(id);
        }


        public new static MimeType ForName(string name)
        {
            return BaseEnum<MimeType>.ForName(name);
        }


        public static MimeType ForMetaType(string appName)
        {
            if (!META_TYPE_MAP.TryGetValue(appName, out var obj))
                throw new ArgumentException($"枚举AppName不存在 -> {appName}");
            return obj;
        }
    }

    public abstract class MimeType<T> : MimeType where T : MimeType<T>, new()
    {
        protected static T Of(int id, string metaType, Action<T> builder = null)
        {
            return E(id, new T {
                MetaType = metaType
            }, builder);
        }

        public new static void LoadAll() => LoadAll(typeof(T));
        
        public new static IReadOnlyCollection<MimeType> GetValues()
        {
            LoadAll(typeof(T));
            return BaseEnum<MimeType>.GetValues();
        }
    }

}
