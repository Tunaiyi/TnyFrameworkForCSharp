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
using TnyFramework.Common.Enum;

namespace TnyFramework.Net.Base
{

    public interface IMessagerType : IEnum
    {
        /// <summary>
        /// 用户组
        /// </summary>
        string Group { get; }
    }

    public class MessagerType : BaseEnum<MessagerType>, IMessagerType
    {
        public const string DEFAULT_USER_TYPE = "#user";

        public const string ANONYMITY_USER_TYPE = "#anonymity";

        private static readonly ConcurrentDictionary<string, MessagerType> GROUP_MAP = new ConcurrentDictionary<string, MessagerType>();

        /// <summary>
        /// 用户组 
        /// </summary>
        public string Group { get; protected set; }

        protected override void OnCheck()
        {
            if (GROUP_MAP.TryAdd(Group, this))
                return;
            var value = GROUP_MAP[Group];
            if (!ReferenceEquals(value, this))
            {
                throw new ArgumentException($"{value} 与 {this} 存在相同的 Group {Group}");
            }
        }

        public new static MessagerType ForId(int id)
        {
            return BaseEnum<MessagerType>.ForId(id);
        }

        public new static MessagerType ForName(string name)
        {
            return BaseEnum<MessagerType>.ForName(name);
        }

        public static MessagerType ForGroup(string group)
        {
            if (!GROUP_MAP.TryGetValue(group, out var obj))
                throw new ArgumentException($"枚举Group不存在 -> {group}");
            return obj;
        }

        public static implicit operator int(MessagerType type) => type.Id;

        public static explicit operator MessagerType(int type) => ForId(type);
        
    }

    public abstract class MessagerType<T> : MessagerType where T : MessagerType<T>, new()
    {
        protected static T Of(int id, string group, Action<T> builder = null)
        {
            return E(id, new T {
                Group = group
            }, builder);
        }

        public new static void LoadAll() => LoadAll(typeof(T));

        public new static IReadOnlyCollection<MessagerType> GetValues()
        {
            LoadAll(typeof(T));
            return BaseEnum<MessagerType>.GetValues();
        }
    }

}
