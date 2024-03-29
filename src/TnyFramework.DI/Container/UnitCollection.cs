// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using TnyFramework.DI.Units;

namespace TnyFramework.DI.Container;

public class UnitCollection<TUnit> : IUnitCollection<TUnit>
{
    public IDictionary<string, TUnit> UnitMap { get; }

    public UnitCollection(IServiceProvider serviceProvider)
    {
        var units = serviceProvider.GetServices<IUnit<TUnit>>();
        var enumerable = units.ToList();
        var instanceMap = new Dictionary<string, TUnit>();
        foreach (var unit in enumerable)
        {
            var instance = unit.Value(serviceProvider);
            instanceMap.Add(unit.Name, instance);
        }
        UnitMap = instanceMap.ToImmutableDictionary();
    }

    public IEnumerator<KeyValuePair<string, TUnit>> GetEnumerator()
    {
        return UnitMap.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return UnitMap.GetEnumerator();
    }

    public void Add(KeyValuePair<string, TUnit> item)
    {
        UnitMap.Add(item);
    }

    public void Clear()
    {
        UnitMap.Clear();
    }

    public bool Contains(KeyValuePair<string, TUnit> item)
    {
        return UnitMap.Contains(item);
    }

    public void CopyTo(KeyValuePair<string, TUnit>[] array, int arrayIndex)
    {
        UnitMap.CopyTo(array, arrayIndex);
    }

    public bool Remove(KeyValuePair<string, TUnit> item)
    {
        return UnitMap.Remove(item);
    }

    public int Count => UnitMap.Count;

    public bool IsReadOnly => UnitMap.IsReadOnly;

    public void Add(string key, TUnit value)
    {
        UnitMap.Add(key, value);
    }

    public bool ContainsKey(string key)
    {
        return UnitMap.ContainsKey(key);
    }

    public bool Remove(string key)
    {
        return UnitMap.Remove(key);
    }

    public bool TryGetValue(string key, out TUnit value)
    {
        return UnitMap.TryGetValue(key, out value!);
    }

    public TUnit this[string key] {
        get => UnitMap[key];
        set => UnitMap[key] = value;
    }

    public ICollection<string> Keys => UnitMap.Keys;

    public ICollection<TUnit> Values => UnitMap.Values;
}
