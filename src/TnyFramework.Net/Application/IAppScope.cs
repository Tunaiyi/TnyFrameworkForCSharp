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

namespace TnyFramework.Net.Application;

public interface IAppScope : IEnum
{
    /// <summary>
    /// 引用名
    /// </summary>
    string ScopeName { get; }
}

public class AppScope : BaseEnum<AppScope>, IAppScope
{
    private static readonly ConcurrentDictionary<string, AppScope> SCOPE_NAME_MAP = new ConcurrentDictionary<string, AppScope>();

    /// <summary>
    /// 区域名字
    /// </summary>
    public string ScopeName { get; protected set; } = "";

    protected override void OnCheck()
    {
        if (SCOPE_NAME_MAP.TryAdd(ScopeName, this))
            return;
        var value = SCOPE_NAME_MAP[ScopeName];
        if (!ReferenceEquals(value, this))
        {
            throw new ArgumentException($"{value} 与 {this} 存在相同的 ScopeName {ScopeName}");
        }
    }

    public new static AppScope ForId(int id)
    {
        return BaseEnum<AppScope>.ForId(id);
    }

    public new static AppScope ForName(string name)
    {
        return BaseEnum<AppScope>.ForName(name);
    }

    public static AppScope ForScopeName(string scopeName)
    {
        if (!SCOPE_NAME_MAP.TryGetValue(scopeName, out var obj))
            throw new ArgumentException($"枚举ScopeName不存在 -> {scopeName}");
        return obj;
    }

    public static implicit operator int(AppScope type) => type.Id;

    public static explicit operator AppScope(int type) => ForId(type);
}

public abstract class AppScope<T> : AppScope where T : AppScope<T>, new()
{
    protected static IAppScope Of(int id, string scopeName, Action<T>? builder = null)
    {
        return E(id, new T {
            ScopeName = scopeName
        }, builder);
    }

    public new static void LoadAll() => LoadAll(typeof(T));

    public new static IReadOnlyCollection<AppScope> GetValues()
    {
        LoadAll(typeof(T));
        return BaseEnum<AppScope>.GetValues();
    }
}
