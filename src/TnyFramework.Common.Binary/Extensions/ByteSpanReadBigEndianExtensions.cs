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

public static partial class ByteSpanReadExtensions
{
    public static Span<byte> ReadDoubleBigEndian(this Span<byte> destination, out double value)
    {
#if NETSTANDARD || NETFRAMEWORK
        value = !BitConverter.IsLittleEndian
            ? MemoryMarshal.Read<double>(destination)
            : BitConverter.Int64BitsToDouble(BinaryPrimitives.ReverseEndianness(MemoryMarshal.Read<long>(destination)));
#else
        value = BinaryPrimitives.ReadDoubleBigEndian(destination);
#endif
        return destination.Slice(sizeof(double));
    }

    public static Span<byte> ReadInt16BigEndian(this Span<byte> destination, out short value)
    {
        value = BinaryPrimitives.ReadInt16BigEndian(destination);
        return destination.Slice(sizeof(short));
    }

    public static Span<byte> ReadInt32BigEndian(this Span<byte> destination, out int value)
    {
        value = BinaryPrimitives.ReadInt32BigEndian(destination);
        return destination.Slice(sizeof(int));
    }

    public static Span<byte> ReadInt64BigEndian(this Span<byte> destination, out long value)
    {
        value = BinaryPrimitives.ReadInt64BigEndian(destination);
        return destination.Slice(sizeof(long));
    }

    public static Span<byte> ReadSingleBigEndian(this Span<byte> destination, out float value)
    {
#if NETSTANDARD || NETFRAMEWORK
        value = !BitConverter.IsLittleEndian
            ? MemoryMarshal.Read<float>(destination)
            : Int32BitsToSingle(BinaryPrimitives.ReverseEndianness(MemoryMarshal.Read<int>(destination)));
#else
        value = BinaryPrimitives.ReadSingleBigEndian(destination);
#endif
        return destination.Slice(sizeof(double));
    }

    public static Span<byte> ReadUInt16BigEndian(this Span<byte> destination, out ushort value)
    {
        value = BinaryPrimitives.ReadUInt16BigEndian(destination);
        return destination.Slice(sizeof(ushort));
    }

    public static Span<byte> ReadUInt32BigEndian(this Span<byte> destination, out uint value)
    {
        value = BinaryPrimitives.ReadUInt32BigEndian(destination);
        return destination.Slice(sizeof(uint));
    }

    public static Span<byte> ReadUInt64BigEndian(this Span<byte> destination, out ulong value)
    {
        value = BinaryPrimitives.ReadUInt64BigEndian(destination);
        return destination.Slice(sizeof(ulong));
    }
}
