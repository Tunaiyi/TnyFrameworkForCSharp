// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;

namespace TnyFramework.FreeRedis.Hosting.Attributes;

/// <summary>
/// Redis 特性
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
public class RedisObjectAttribute : Attribute
{
    /// <summary>
    /// 数据源
    /// </summary>
    public string Source { get; }

    /// <summary>
    /// 媒体类型
    /// </summary>
    public string Mime { get; }

    public RedisObjectAttribute(string mime, string source = "")
    {
        Source = source;
        Mime = mime;
    }
}
