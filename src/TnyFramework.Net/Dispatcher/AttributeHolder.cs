using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace TnyFramework.Net.Dispatcher
{

    public class AttributeHolder
    {
        /// <summary>
        /// 类型 : 特性列表
        /// </summary>
        public IDictionary<Type, IList<Attribute>> AttributesMap { get; private set; }

        /// <summary>
        /// 类型 : 特性列表
        /// </summary>
        private IDictionary<Type, IList> AttributesWithTypeMap { get; } = new Dictionary<Type, IList>();

        /// <summary>
        /// 所有特性列表
        /// </summary>
        public IList<Attribute> Attributes { get; private set; }

        public bool Empty { get; private set; } = true;

        public AttributeHolder(IEnumerable<Attribute> attributes)
        {
            Init(attributes);
        }

        public AttributeHolder(MemberInfo member)
        {
            Init(member.GetCustomAttributes());
        }

        public void Init(IEnumerable<Attribute> attributes)
        {
            var map = new Dictionary<Type, IList<Attribute>>();
            if (attributes == null)
            {
                throw new NullReferenceException();
            }
            var attributeList = attributes.ToList();
            foreach (var attr in attributeList)
            {
                Empty = false;
                if (!map.TryGetValue(attr.GetType(), out var list))
                {
                    list = new List<Attribute>();
                    map.Add(attr.GetType(), list);
                }
                list.Add(attr);
            }
            AttributesMap = map.ToDictionary<KeyValuePair<Type, IList<Attribute>>, Type, IList<Attribute>>(
                    keyValuePair => keyValuePair.Key,
                    keyValuePair => ImmutableList.CreateRange(keyValuePair.Value))
                .ToImmutableDictionary();
            Attributes = ImmutableList.CreateRange(attributeList);
        }

        public Attribute GetAttribute(Type attributeType)
        {
            var attributes = AttributesMap[attributeType];
            if (attributes == null || attributes.Count == 0)
            {
                return null;
            }
            return (Attribute) attributes[0];
        }

        public TAttribute GetAttribute<TAttribute>() where TAttribute : Attribute
        {
            if (!AttributesMap.TryGetValue(typeof(TAttribute), out var attributes))
            {
                return null;
            }
            if (attributes == null || attributes.Count == 0)
            {
                return null;
            }
            return (TAttribute) attributes[0];
        }

        public IList<Attribute> GetAttributes(Type attributeType)
        {
            var attributes = AttributesMap[attributeType];
            if (attributes == null || attributes.Count == 0)
            {
                return ImmutableList.Create<Attribute>();
            }
            return attributes;
        }

        public IList<TAttribute> GetAttributes<TAttribute>() where TAttribute : Attribute
        {
            var type = typeof(TAttribute);
            var attributes = AttributesWithTypeMap[type];
            if (attributes != null)
            {
                return (IList<TAttribute>) attributes;
            }
            lock (this)
            {
                attributes = AttributesWithTypeMap[type];
                if (attributes != null)
                {
                    return (IList<TAttribute>) attributes;
                }
                var source = AttributesMap[type];
                if (source == null || source.Count == 0)
                {
                    return ImmutableList.Create<TAttribute>();
                }
                var tAttributes = new List<TAttribute>();
                foreach (var attribute in source)
                {
                    tAttributes.Add((TAttribute) attribute);
                }
                var newOne = ImmutableList.CreateRange(tAttributes);
                AttributesWithTypeMap[type] = newOne;
                return newOne;
            }
        }

        public ICollection<Type> AttributeTypes => AttributesMap.Keys;
    }

}
