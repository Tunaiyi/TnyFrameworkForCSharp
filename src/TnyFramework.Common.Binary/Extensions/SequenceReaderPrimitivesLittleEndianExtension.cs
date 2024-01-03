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
        public static double ReadDoubleLittleEndian(this ref SequenceReader<byte> reader)
        {
            Span<byte> span = stackalloc byte[sizeof(double)];
            if (!reader.TryCopyTo(span))
            {
                throw new IndexOutOfRangeException();
            }
            reader.Advance(span.Length);
            return span.ReadDoubleLittleEndian();
        }

        public static short ReadInt16LittleEndian(this ref SequenceReader<byte> reader)
        {
            Span<byte> span = stackalloc byte[sizeof(short)];
            if (!reader.TryCopyTo(span))
            {
                throw new IndexOutOfRangeException();
            }
            reader.Advance(span.Length);
            return BinaryPrimitives.ReadInt16LittleEndian(span);
        }

        public static int ReadInt32LittleEndian(this ref SequenceReader<byte> reader)
        {
            Span<byte> span = stackalloc byte[sizeof(int)];
            if (!reader.TryCopyTo(span))
            {
                throw new IndexOutOfRangeException();
            }
            reader.Advance(span.Length);
            return BinaryPrimitives.ReadInt32LittleEndian(span);
        }

        public static long ReadInt64LittleEndian(this ref SequenceReader<byte> reader)
        {
            Span<byte> span = stackalloc byte[sizeof(long)];
            if (!reader.TryCopyTo(span))
            {
                throw new IndexOutOfRangeException();
            }
            reader.Advance(span.Length);
            return BinaryPrimitives.ReadInt64LittleEndian(span);
        }

        public static float ReadSingleLittleEndian(this ref SequenceReader<byte> reader)
        {
            Span<byte> span = stackalloc byte[sizeof(float)];
            if (!reader.TryCopyTo(span))
            {
                throw new IndexOutOfRangeException();
            }
            reader.Advance(span.Length);
            return span.ReadSingleLittleEndian();
        }

        public static ushort ReadUInt16LittleEndian(this ref SequenceReader<byte> reader)
        {
            Span<byte> span = stackalloc byte[sizeof(ushort)];
            if (!reader.TryCopyTo(span))
            {
                throw new IndexOutOfRangeException();
            }
            reader.Advance(span.Length);
            return BinaryPrimitives.ReadUInt16LittleEndian(span);
        }

        public static uint ReadUInt32LittleEndian(this ref SequenceReader<byte> reader)
        {
            Span<byte> span = stackalloc byte[sizeof(uint)];
            if (!reader.TryCopyTo(span))
            {
                throw new IndexOutOfRangeException();
            }
            reader.Advance(span.Length);
            return BinaryPrimitives.ReadUInt32LittleEndian(span);
        }

        public static ulong ReadUInt64LittleEndian(this ref SequenceReader<byte> reader)
        {
            Span<byte> span = stackalloc byte[sizeof(ulong)];
            if (!reader.TryCopyTo(span))
            {
                throw new IndexOutOfRangeException();
            }
            reader.Advance(span.Length);
            return BinaryPrimitives.ReadUInt64LittleEndian(span);
        }
    }

}
#endif
