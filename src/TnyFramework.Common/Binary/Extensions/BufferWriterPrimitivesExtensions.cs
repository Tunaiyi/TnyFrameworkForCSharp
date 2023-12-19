// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Buffers;
using System.Runtime.InteropServices;
using System.Text;

namespace TnyFramework.Common.Binary.Extensions
{

    public static partial class BufferWriterPrimitivesExtensions
    {
        private static Span<byte> Span<T>(this IBufferWriter<byte> writer) where T : struct
        {
            var size = Marshal.SizeOf(typeof(T));
            var span = writer.GetSpan(size);
            writer.Advance(size);
            return span;
        }

        public static IBufferWriter<byte> WriteByte(this IBufferWriter<byte> writer, byte value)
        {
            writer.Span<byte>().WriteByte(value);
            return writer;
        }

        public static IBufferWriter<byte> WriteByte(this IBufferWriter<byte> writer, sbyte value)
        {
            writer.Span<sbyte>().WriteByte((byte) value);
            return writer;
        }

        public static void WriteBytes(this IBufferWriter<byte> writer, ReadOnlySequence<byte> source)
        {
            foreach (var memory in source)
            {
                writer.Write(memory.Span);
            }
        }

        public static int WriteString(this IBufferWriter<byte> writer, string value, Encoding? encoding = null)
        {
            encoding ??= Encoding.UTF8;
            return GetBytes(encoding, value, writer);
        }

        private static unsafe int GetBytes(Encoding encoding, string value, IBufferWriter<byte> writer)
        {
#if NET
            return (int) encoding.GetBytes(value, writer);
#else
            int byteCount = encoding.GetByteCount(value);
            Span<byte> buffer = writer.GetSpan(byteCount);
            ReadOnlySpan<char> chars = value.ToCharArray();
            fixed (char* charsPtr = &MemoryMarshal.GetReference(chars))
            fixed (byte* bytesPtr = &MemoryMarshal.GetReference(buffer))
            {
                int length = encoding.GetBytes(charsPtr, value.Length, bytesPtr, buffer.Length);
                if (length > 0) {
                    writer.Advance(byteCount);
                }
                return length;
            }
#endif
        }
    }

}
