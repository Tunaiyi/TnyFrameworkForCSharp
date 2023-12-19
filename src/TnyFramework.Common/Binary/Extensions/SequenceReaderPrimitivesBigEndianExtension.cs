// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

#if NET
using System;
using System.Buffers;
using System.Buffers.Binary;

namespace TnyFramework.Common.Binary.Extensions
{

    public static partial class SequenceReaderPrimitivesExtension
    {
        public static double ReadDoubleBigEndian(this ref SequenceReader<byte> reader)
        {
            Span<byte> span = stackalloc byte[sizeof(double)];
            if (!reader.TryCopyTo(span))
            {
                throw new IndexOutOfRangeException();
            }
            reader.Advance(span.Length);
            span.ReadDoubleBigEndian(out var value);
            return value;
        }

        public static short ReadInt16BigEndian(this ref SequenceReader<byte> reader)
        {
            Span<byte> span = stackalloc byte[sizeof(short)];
            if (!reader.TryCopyTo(span))
            {
                throw new IndexOutOfRangeException();
            }
            reader.Advance(span.Length);
            return BinaryPrimitives.ReadInt16BigEndian(span);
        }

        public static int ReadInt32BigEndian(this ref SequenceReader<byte> reader)
        {
            Span<byte> span = stackalloc byte[sizeof(int)];
            if (!reader.TryCopyTo(span))
            {
                throw new IndexOutOfRangeException();
            }
            reader.Advance(span.Length);
            return BinaryPrimitives.ReadInt32BigEndian(span);
        }

        public static long ReadInt64BigEndian(this ref SequenceReader<byte> reader)
        {
            Span<byte> span = stackalloc byte[sizeof(long)];
            if (!reader.TryCopyTo(span))
            {
                throw new IndexOutOfRangeException();
            }
            reader.Advance(span.Length);
            return BinaryPrimitives.ReadInt64BigEndian(span);
        }

        public static float ReadSingleBigEndian(this ref SequenceReader<byte> reader)
        {
            Span<byte> span = stackalloc byte[sizeof(float)];
            if (!reader.TryCopyTo(span))
            {
                throw new IndexOutOfRangeException();
            }
            reader.Advance(span.Length);
            span.ReadSingleBigEndian(out var value);
            return value;
        }

        public static ushort ReadUInt16BigEndian(this ref SequenceReader<byte> reader)
        {
            Span<byte> span = stackalloc byte[sizeof(ushort)];
            if (!reader.TryCopyTo(span))
            {
                throw new IndexOutOfRangeException();
            }
            reader.Advance(span.Length);
            return BinaryPrimitives.ReadUInt16BigEndian(span);
        }

        public static uint ReadUInt32BigEndian(this ref SequenceReader<byte> reader)
        {
            Span<byte> span = stackalloc byte[sizeof(uint)];
            if (!reader.TryCopyTo(span))
            {
                throw new IndexOutOfRangeException();
            }
            reader.Advance(span.Length);
            return BinaryPrimitives.ReadUInt32BigEndian(span);
        }

        public static ulong ReadUInt64BigEndian(this ref SequenceReader<byte> reader)
        {
            Span<byte> span = stackalloc byte[sizeof(ulong)];
            if (!reader.TryCopyTo(span))
            {
                throw new IndexOutOfRangeException();
            }
            reader.Advance(span.Length);
            return BinaryPrimitives.ReadUInt64BigEndian(span);
        }
    }

}
#endif
