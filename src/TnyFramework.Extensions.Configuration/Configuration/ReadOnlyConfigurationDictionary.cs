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
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace TnyFramework.Extensions.Configuration.Configuration;

public class ReadOnlyConfigurationDictionary(IConfiguration configuration) : IReadOnlyDictionary<string, string?>
{
    public IEnumerator<KeyValuePair<string, string?>> GetEnumerator() => configuration.AsEnumerable().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public int Count => configuration.AsEnumerable().Count();

    public bool ContainsKey(string key)
    {
        return configuration[key] != null;
    }

    public bool TryGetValue(string key, out string? value)
    {
        value = configuration[key];
        return value != null;
    }

    public string? this[string key] {
        get => configuration[key];
        set => throw new NotImplementedException();
    }

    public IEnumerable<string> Keys => configuration.AsEnumerable().Select(x => x.Key).ToList();

    public IEnumerable<string?> Values => configuration.AsEnumerable().Select(x => x.Value).ToList();
}
