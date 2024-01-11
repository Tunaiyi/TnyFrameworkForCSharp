// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace TnyFramework.Net.Extensions;

public static class UriExtensions
{
    public static IDictionary<string, string> GetQueryParameters(this Uri uri)
    {
        var query = uri.Query;
        if (string.IsNullOrEmpty(query))
        {
            return new Dictionary<string, string>();
        }
        query = query.Substring(1); // 去除查询字符串开头的问号
        return query.Split('&')
            .Select(pair => pair.Split('='))
            .Where(parts => parts.Length == 2)
            .ToDictionary(
                parts => WebUtility.UrlDecode(parts[0]),
                parts => WebUtility.UrlDecode(parts[1]));
    }

    public static string[] GetPathSegments(this Uri uri)
    {
        return uri.AbsolutePath.Split(new[] {'/'}, StringSplitOptions.RemoveEmptyEntries);
    }
}
