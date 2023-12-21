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

    public static partial class ByteSpanWriteExtensions
    {
        public static Span<byte> WriteDoubleBigEndian(this Span<byte> destination, double value)
        {
#if NETSTANDARD || NETFRAMEWORK
            if (!BitConverter.IsLittleEndian)
            {
                MemoryMarshal.Write(destination, ref value);
            } else
            {
                var tmp = BinaryPrimitives.ReverseEndianness(BitConverter.DoubleToInt64Bits(value));
                MemoryMarshal.Write(destination, ref tmp);
            }
#else
            BinaryPrimitives.WriteDoubleBigEndian(destination, value);
#endif
            return destination.Slice(sizeof(double));
        }

        public static Span<byte> WriteInt16BigEndian(this Span<byte> destination, short value)
        {
            BinaryPrimitives.WriteInt16BigEndian(destination, value);
            return destination.Slice(sizeof(short));
        }

        public static Span<byte> WriteInt32BigEndian(this Span<byte> destination, int value)
        {
            BinaryPrimitives.WriteInt32BigEndian(destination, value);
            return destination.Slice(sizeof(int));
        }

        public static Span<byte> WriteInt64BigEndian(this Span<byte> destination, long value)
        {
            BinaryPrimitives.WriteInt64BigEndian(destination, value);
            return destination.Slice(sizeof(long));
        }

        public static Span<byte> WriteSingleBigEndian(this Span<byte> destination, float value)
        {
#if NETSTANDARD || NETFRAMEWORK
            if (!BitConverter.IsLittleEndian)
            {
                MemoryMarshal.Write(destination, ref value);
            } else
            {
                var tmp = BinaryPrimitives.ReverseEndianness(SingleToInt32Bits(value));
                MemoryMarshal.Write(destination, ref tmp);
            }
#else
            BinaryPrimitives.WriteSingleBigEndian(destination, value);
#endif
            return destination.Slice(sizeof(double));
        }

        public static Span<byte> WriteUInt16BigEndian(this Span<byte> destination, ushort value)
        {
            BinaryPrimitives.WriteUInt16BigEndian(destination, value);
            return destination.Slice(sizeof(ushort));
        }

        public static Span<byte> WriteUInt32BigEndian(this Span<byte> destination, uint value)
        {
            BinaryPrimitives.WriteUInt32BigEndian(destination, value);
            return destination.Slice(sizeof(uint));
        }

        public static Span<byte> WriteUInt64BigEndian(this Span<byte> destination, ulong value)
        {
            BinaryPrimitives.WriteUInt64BigEndian(destination, value);
            return destination.Slice(sizeof(ulong));
        }
    }

}
