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

namespace TnyFramework.Codec;

/// <summary>
/// 媒体类型
/// </summary>
public interface IMimeType : IEnum
{
    string MetaType { get; }
}

public class MimeType : BaseEnum<MimeType>, IMimeType
{
    private static readonly ConcurrentDictionary<string, MimeType> MIME_TYPE_MAP = new ConcurrentDictionary<string, MimeType>();

    public string MetaType { get; protected set; } = "";

    protected override void OnCheck()
    {
        if (MIME_TYPE_MAP.TryAdd(MetaType, this))
            return;
        var value = MIME_TYPE_MAP[MetaType];
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

    public static MimeType ForMimeType(string mime)
    {
        if (!MIME_TYPE_MAP.TryGetValue(mime, out var obj))
            throw new ArgumentException($"枚举AppName不存在 -> {mime}");
        return obj;
    }

    public static implicit operator int(MimeType type) => type.Id;

    public static explicit operator MimeType(int type) => ForId(type);
}

public abstract class MimeType<T> : MimeType where T : MimeType<T>, new()
{
    protected static T Of(int id, string metaType, Action<T>? builder = null)
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
