// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using TnyFramework.Common.Exceptions;

namespace TnyFramework.Extensions.Configuration.Configuration;

public static class ConfigurationExtensions
{
    public static IReadOnlyDictionary<string, string?> AsReadOnlyDictionary(this IConfiguration configuration)
    {
        return new ReadOnlyConfigurationDictionary(configuration);
    }

    public static TOptions? BindOptionsIfExists<TOptions>(this IConfiguration configuration, string path)
        where TOptions : new()
    {
        if (!configuration.GetSection(path).Exists())
        {
            return default;
        }
        var options = new TOptions();
        configuration.Bind(path, options);
        return options;
    }

    public static TOptions BindOptions<TOptions>(this IConfiguration configuration, string path, bool required = true)
        where TOptions : new()
    {
        var options = new TOptions();
        if (!configuration.GetSection(path).Exists())
        {
            if (required)
            {
                throw new IllegalArgumentException($"{typeof(TOptions)} 未配置，配置路径 : {path}");
            }
            return options;
        }
        configuration.Bind(path, options);
        return options;
    }

    public static TOptions BindOptions<TOptions>(this IConfiguration configuration, TOptions options, string path,
        bool required = true)
        where TOptions : new()
    {
        if (!configuration.GetSection(path).Exists())
        {
            if (required)
            {
                throw new IllegalArgumentException($"{typeof(TOptions)} 未配置，配置路径 : {path}");
            }
            return options;
        }
        configuration.Bind(path, options);
        return options;
    }
}
