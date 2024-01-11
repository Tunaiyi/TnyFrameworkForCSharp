// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Buffers.Binary;
using System.Runtime.InteropServices;

namespace TnyFramework.Common.Binary.Extensions;

public static partial class ByteSpanWriteExtensions
{
    public static Span<byte> WriteDoubleLittleEndian(this Span<byte> destination, double value)
    {

#if NETSTANDARD || NETFRAMEWORK
        if (!BitConverter.IsLittleEndian)
        {
            var tmp = BinaryPrimitives.ReverseEndianness(BitConverter.DoubleToInt64Bits(value));
            MemoryMarshal.Write(destination, ref tmp);
        } else
        {
            MemoryMarshal.Write(destination, ref value);
        }
#else
        BinaryPrimitives.WriteDoubleLittleEndian(destination, value);
#endif
        return destination.Slice(sizeof(double));
    }

    public static Span<byte> WriteInt16LittleEndian(this Span<byte> destination, short value)
    {
        BinaryPrimitives.WriteInt16LittleEndian(destination, value);
        return destination.Slice(sizeof(short));
    }

    public static Span<byte> WriteInt32LittleEndian(this Span<byte> destination, int value)
    {
        BinaryPrimitives.WriteInt32LittleEndian(destination, value);
        return destination.Slice(sizeof(int));
    }

    public static Span<byte> WriteInt64LittleEndian(this Span<byte> destination, long value)
    {
        BinaryPrimitives.WriteInt64LittleEndian(destination, value);
        return destination.Slice(sizeof(long));
    }

    public static Span<byte> WriteSingleLittleEndian(this Span<byte> destination, float value)
    {
#if NETSTANDARD || NETFRAMEWORK
        if (!BitConverter.IsLittleEndian)
        {
            var tmp = BinaryPrimitives.ReverseEndianness(SingleToInt32Bits(value));
            MemoryMarshal.Write(destination, ref tmp);
        } else
        {
            MemoryMarshal.Write(destination, ref value);
        }
#else
        BinaryPrimitives.WriteSingleLittleEndian(destination, value);
#endif
        return destination.Slice(sizeof(double));
    }

    public static Span<byte> WriteUInt16LittleEndian(this Span<byte> destination, ushort value)
    {
        BinaryPrimitives.WriteUInt16LittleEndian(destination, value);
        return destination.Slice(sizeof(ushort));
    }

    public static Span<byte> WriteUInt32LittleEndian(this Span<byte> destination, uint value)
    {
        BinaryPrimitives.WriteUInt32LittleEndian(destination, value);
        return destination.Slice(sizeof(uint));
    }

    public static Span<byte> WriteUInt64LittleEndian(this Span<byte> destination, ulong value)
    {
        BinaryPrimitives.WriteUInt64LittleEndian(destination, value);
        return destination.Slice(sizeof(ulong));
    }
}
