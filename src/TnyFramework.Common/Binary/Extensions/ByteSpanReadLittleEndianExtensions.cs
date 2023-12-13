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

namespace TnyFramework.Common.Binary.Extensions
{

    public static partial class ByteSpanReadExtensions
    {
        public static Span<byte> ReadDoubleLittleEndian(this Span<byte> destination, out double value)
        {
#if NETSTANDARD || NETFRAMEWORK
            value = !BitConverter.IsLittleEndian
                ? BitConverter.Int64BitsToDouble(BinaryPrimitives.ReverseEndianness(MemoryMarshal.Read<long>(destination)))
                : MemoryMarshal.Read<double>(destination);
#else
            value = BinaryPrimitives.ReadDoubleLittleEndian(destination);
#endif
            return destination.Slice(sizeof(double));
        }

        public static Span<byte> ReadInt16LittleEndian(this Span<byte> destination, out short value)
        {
            value = BinaryPrimitives.ReadInt16LittleEndian(destination);
            return destination.Slice(sizeof(short));
        }

        public static Span<byte> ReadInt32LittleEndian(this Span<byte> destination, out int value)
        {
            value = BinaryPrimitives.ReadInt32LittleEndian(destination);
            return destination.Slice(sizeof(int));
        }

        public static Span<byte> ReadInt64LittleEndian(this Span<byte> destination, out long value)
        {
            value = BinaryPrimitives.ReadInt64LittleEndian(destination);
            return destination.Slice(sizeof(long));
        }

        public static Span<byte> ReadSingleLittleEndian(this Span<byte> destination, out float value)
        {
#if NETSTANDARD || NETFRAMEWORK
            value = !BitConverter.IsLittleEndian
                ? BitConverter.Int32BitsToSingle(BinaryPrimitives.ReverseEndianness(MemoryMarshal.Read<int>(destination)))
                : MemoryMarshal.Read<float>(destination);
#else
            value = BinaryPrimitives.ReadSingleLittleEndian(destination);
#endif
            return destination.Slice(sizeof(double));
        }

        public static Span<byte> ReadUInt16LittleEndian(this Span<byte> destination, out ushort value)
        {
            value = BinaryPrimitives.ReadUInt16LittleEndian(destination);
            return destination.Slice(sizeof(ushort));
        }

        public static Span<byte> ReadUInt32LittleEndian(this Span<byte> destination, out uint value)
        {
            value = BinaryPrimitives.ReadUInt32LittleEndian(destination);
            return destination.Slice(sizeof(uint));
        }

        public static Span<byte> ReadUInt64LittleEndian(this Span<byte> destination, out ulong value)
        {
            value = BinaryPrimitives.ReadUInt64LittleEndian(destination);
            return destination.Slice(sizeof(ulong));
        }

        public static double ReadDoubleLittleEndian(this Span<byte> destination)
        {
#if NETSTANDARD || NETFRAMEWORK
            return !BitConverter.IsLittleEndian
                ? BitConverter.Int64BitsToDouble(BinaryPrimitives.ReverseEndianness(MemoryMarshal.Read<long>(destination)))
                : MemoryMarshal.Read<double>(destination);
#else
            return BinaryPrimitives.ReadDoubleLittleEndian(destination);
#endif
        }

        public static short ReadInt16LittleEndian(this Span<byte> destination)
        {
            return BinaryPrimitives.ReadInt16LittleEndian(destination);
        }

        public static int ReadInt32LittleEndian(this Span<byte> destination)
        {
            return BinaryPrimitives.ReadInt32LittleEndian(destination);
        }

        public static long ReadInt64LittleEndian(this Span<byte> destination)
        {
            return BinaryPrimitives.ReadInt64LittleEndian(destination);
        }

        public static float ReadSingleLittleEndian(this Span<byte> destination)
        {
#if NETSTANDARD || NETFRAMEWORK
            return !BitConverter.IsLittleEndian
                ? BitConverter.Int32BitsToSingle(BinaryPrimitives.ReverseEndianness(MemoryMarshal.Read<int>(destination)))
                : MemoryMarshal.Read<float>(destination);
#else
            return BinaryPrimitives.ReadSingleLittleEndian(destination);
#endif
        }

        public static ushort ReadUInt16LittleEndian(this Span<byte> destination)
        {
            return BinaryPrimitives.ReadUInt16LittleEndian(destination);
        }

        public static uint ReadUInt32LittleEndian(this Span<byte> destination)
        {
            return BinaryPrimitives.ReadUInt32LittleEndian(destination);
        }

        public static ulong ReadUInt64LittleEndian(this Span<byte> destination)
        {
            return BinaryPrimitives.ReadUInt64LittleEndian(destination);
        }
    }

}
