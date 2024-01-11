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
using System.Collections.Immutable;

namespace TnyFramework.Codec;

public abstract class ObjectCodecFactory : IObjectCodecFactory
{
    private readonly ConcurrentDictionary<Type, IObjectCodec> codecs = new ConcurrentDictionary<Type, IObjectCodec>();

    public IObjectCodec<T> CreateCodec<T>()
    {
        return (IObjectCodec<T>) codecs.GetOrAdd(typeof(T), _ => Create<T>());
    }

    public IObjectCodec CreateCodec(Type type)
    {
        return codecs.GetOrAdd(type, Create);
    }

    public IReadOnlyList<IMimeType> MediaTypes { get; }

    public ObjectCodecFactory(IMimeType mimeType, params IMimeType[] mediaTypes)
    {
        var types = new List<IMimeType> {mimeType};
        types.AddRange(mediaTypes);
        MediaTypes = types.ToImmutableList();
    }

    protected abstract IObjectCodec<T> Create<T>();

    protected abstract IObjectCodec Create(Type type);
}
