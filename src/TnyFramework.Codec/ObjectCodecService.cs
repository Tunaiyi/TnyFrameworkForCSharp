// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;

namespace TnyFramework.Codec;

/// <summary>
/// 对象编解码服务
/// </summary>
public class ObjectCodecService
{
    private readonly ObjectCodecAdapter objectCodecAdapter;

    public ObjectCodecService(ObjectCodecAdapter objectCodecAdapter)
    {
        this.objectCodecAdapter = objectCodecAdapter;
    }

    public bool IsSupported(IMimeType type)
    {
        return objectCodecAdapter.IsSupported(type);
    }

    public IObjectCodec<T> Codec<T>(ObjectMimeType<T> objectMimeType)
    {
        return objectCodecAdapter.Codec(objectMimeType);
    }

    public IObjectCodec Codec(Type objectType, IMimeType mimeType)
    {
        return objectCodecAdapter.Codec(objectType, mimeType);
    }

    public IObjectCodec<T> Codec<T>()
    {
        return objectCodecAdapter.Codec<T>();
    }

    public IObjectCodec Codec(Type objectType)
    {
        return objectCodecAdapter.Codec(objectType);
    }

    public IObjectCodec<T> Codec<T>(IMimeType mimeType)
    {
        return objectCodecAdapter.Codec<T>(mimeType);
    }

    public byte[] EncodeToBytes(object value)
    {
        return objectCodecAdapter.EncodeToBytes(value);
    }

    public byte[] EncodeToBytes<T>(ObjectMimeType<T> mimeType, T value)
    {
        return objectCodecAdapter.EncodeToBytes(mimeType, value);
    }

    public byte[] EncodeToBytes(object value, IMimeType mineType)
    {
        return objectCodecAdapter.EncodeToBytes(value, mineType);
    }

    public T DecodeByBytes<T>(byte[] data)
    {
        return objectCodecAdapter.DecodeByBytes<T>(data)!;
    }

    public T DecodeByBytes<T>(ObjectMimeType<T> mimeType, byte[] data)
    {
        return objectCodecAdapter.DecodeByBytes(mimeType, data)!;
    }

    public T DecodeByBytes<T>(IMimeType mimeType, byte[] data)
    {
        return objectCodecAdapter.DecodeByBytes<T>(mimeType, data)!;
    }
}
