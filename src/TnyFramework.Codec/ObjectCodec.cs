// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.IO;
#if NET
using System.Buffers;
#endif

namespace TnyFramework.Codec
{

    public abstract class ObjectCodec<T> : IObjectCodec<T>
    {
        public abstract byte[] Encode(T? value);

        public abstract void Encode(T? value, Stream output);

        public abstract T? Decode(byte[]? bytes);

        public abstract T? Decode(Stream input);

        public abstract string? Format(T? value);

        public abstract T? Parse(string? data);

        public byte[] Encode(object? value)
        {
            if (value is T data)
            {
                return Encode(data);
            } else
            {
                throw new InvalidCastException($"{value} 无法转位 {nameof(T)}");
            }
        }

        public void Encode(object? value, Stream output)
        {
            if (value is T data)
            {
                Encode(data, output);
            } else
            {
                throw new InvalidCastException($"{value} 无法转位 {nameof(T)}");
            }
        }

        object? IObjectCodec.Decode(byte[] bytes)
        {
            return Decode(bytes);
        }

        object? IObjectCodec.Decode(Stream input)
        {
            return Decode(input);
        }

        public string? Format(object value)
        {
            if (value is T data)
            {
                return Format(data);
            } else
            {
                throw new InvalidCastException($"{value} 无法转位 {nameof(T)}");
            }
        }

        object? IObjectCodec.Parse(string data)
        {
            return Parse(data);
        }

#if NET
        public abstract void Encode(T? value, IBufferWriter<byte> output);

        public abstract T? Decode(ReadOnlySequence<byte> input);

        object? IObjectCodec.Decode(ReadOnlySequence<byte> input)
        {
            return Decode(input);
        }

        public void Encode(object? value, IBufferWriter<byte> output)
        {
            if (value is T data)
            {
                Encode(data, output);
            } else
            {
                throw new InvalidCastException($"{value} 无法转位 {nameof(T)}");
            }
        }
#endif
    }

}
