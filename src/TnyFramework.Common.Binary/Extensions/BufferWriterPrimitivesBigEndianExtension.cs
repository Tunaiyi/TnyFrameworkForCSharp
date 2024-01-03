// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System.Buffers;

namespace TnyFramework.Common.Binary.Extensions
{

    public static partial class BufferWriterPrimitivesExtensions
    {
        public static IBufferWriter<byte> WriteDoubleLittleEndian(this IBufferWriter<byte> writer, double value)
        {
            writer.Span<double>().WriteDoubleLittleEndian(value);
            return writer;
        }

        public static IBufferWriter<byte> WriteInt16LittleEndian(this IBufferWriter<byte> writer, short value)
        {
            writer.Span<short>().WriteInt16LittleEndian(value);
            return writer;
        }

        public static IBufferWriter<byte> WriteInt32LittleEndian(this IBufferWriter<byte> writer, int value)
        {
            writer.Span<int>().WriteInt32LittleEndian(value);
            return writer;
        }

        public static IBufferWriter<byte> WriteInt64LittleEndian(this IBufferWriter<byte> writer, long value)
        {
            writer.Span<long>().WriteInt64LittleEndian(value);
            return writer;
        }

        public static IBufferWriter<byte> WriteSingleLittleEndian(this IBufferWriter<byte> writer, float value)
        {
            writer.Span<float>().WriteSingleLittleEndian(value);
            return writer;
        }

        public static IBufferWriter<byte> WriteUInt16LittleEndian(this IBufferWriter<byte> writer, ushort value)
        {
            writer.Span<ushort>().WriteUInt16LittleEndian(value);
            return writer;
        }

        public static IBufferWriter<byte> WriteUInt32LittleEndian(this IBufferWriter<byte> writer, uint value)
        {
            writer.Span<uint>().WriteUInt32LittleEndian(value);
            return writer;
        }

        public static IBufferWriter<byte> WriteUInt64LittleEndian(this IBufferWriter<byte> writer, ulong value)
        {
            writer.Span<ulong>().WriteUInt64LittleEndian(value);
            return writer;
        }
    }

}
