// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System.IO;
#if NET
using System.Buffers;
#endif

namespace TnyFramework.Codec
{

    public interface IObjectCodec
    {
        byte[] Encode(object? value);

        void Encode(object? value, Stream output);

        object? Decode(byte[] bytes);

        object? Decode(Stream input);

        string? Format(object value);

        object? Parse(string data);

#if NET
        void Encode(object? value, IBufferWriter<byte> output);

        object? Decode(ReadOnlySequence<byte> input);
#endif
    }

    public interface IObjectCodec<T> : IObjectCodec
    {
        byte[] Encode(T? value);

        void Encode(T? value, Stream output);

        new T? Decode(byte[] bytes);

        new T? Decode(Stream input);

        string? Format(T value);

        new T? Parse(string data);

#if NET
        void Encode(T? value, IBufferWriter<byte> output);

        new T? Decode(ReadOnlySequence<byte> input);
#endif
    }

}
