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

namespace TnyFramework.Net.Application
{

    public interface IAppType : IEnum
    {
        /// <summary>
        /// 引用名
        /// </summary>
        string AppName { get; }
    }

    public class AppType : BaseEnum<AppType>, IAppType
    {
        private static readonly ConcurrentDictionary<string, AppType> APP_NAME_MAP = new();

        /// <summary>
        /// 应用名字
        /// </summary>
        public string AppName { get; protected set; } = "";

        protected override void OnCheck()
        {
            if (APP_NAME_MAP.TryAdd(AppName, this))
                return;
            var value = APP_NAME_MAP[AppName];
            if (!ReferenceEquals(value, this))
            {
                throw new ArgumentException($"{value} 与 {this} 存在相同的 AppName {AppName}");
            }
        }

        public new static AppType ForId(int id)
        {
            return BaseEnum<AppType>.ForId(id);
        }

        public new static AppType ForName(string name)
        {
            return BaseEnum<AppType>.ForName(name);
        }

        public static AppType ForAppName(string appName)
        {
            if (!APP_NAME_MAP.TryGetValue(appName, out var obj))
                throw new ArgumentException($"枚举AppName不存在 -> {appName}");
            return obj;
        }

        public static implicit operator int(AppType type) => type.Id;

        public static explicit operator AppType(int type) => ForId(type);
    }

    public abstract class AppType<T> : AppType where T : AppType<T>, new()
    {
        protected static T Of(int id, string appName, Action<T>? builder = null)
        {
            return E(id, new T {
                AppName = appName
            }, builder);
        }

        public new static void LoadAll() => LoadAll(typeof(T));

        public new static IReadOnlyCollection<AppType> GetValues()
        {
            LoadAll(typeof(T));
            return BaseEnum<AppType>.GetValues();
        }
    }

}
