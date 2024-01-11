// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;

namespace TnyFramework.Common.Binary.Extensions;

public static partial class ByteSpanWriteExtensions
{
    public static Span<byte> WriteByte(this Span<byte> destination, byte value)
    {
        destination[0] = value;
        return destination.Slice(sizeof(byte));
    }

    public static Span<byte> WriteByte(this Span<byte> destination, sbyte value)
    {
        destination[0] = (byte) value;
        return destination.Slice(sizeof(sbyte));
    }

    public static Span<byte> WriteBytes(this Span<byte> destination, byte[] value, int offset, int length)
    {
        value.AsSpan(offset, length).CopyTo(destination);
        return destination;
    }

    public static Span<byte> WriteBytes(this Span<byte> destination, byte[] value)
    {
        value.CopyTo(destination);
        return destination;
    }

    private static unsafe int SingleToInt32Bits(float value) => *(int*) &value;
}