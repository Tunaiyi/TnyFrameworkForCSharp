// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System.Buffers;

namespace TnyFramework.Common.Binary.Extensions;

public static partial class BufferWriterPrimitivesExtensions
{
    public static IBufferWriter<byte> WriteDoubleBigEndian(this IBufferWriter<byte> writer, double value)
    {
        writer.Span<double>().WriteDoubleBigEndian(value);
        return writer;
    }

    public static IBufferWriter<byte> WriteInt16BigEndian(this IBufferWriter<byte> writer, short value)
    {
        writer.Span<short>().WriteInt16BigEndian(value);
        return writer;
    }

    public static IBufferWriter<byte> WriteInt32BigEndian(this IBufferWriter<byte> writer, int value)
    {
        writer.Span<int>().WriteInt32BigEndian(value);
        return writer;
    }

    public static IBufferWriter<byte> WriteInt64BigEndian(this IBufferWriter<byte> writer, long value)
    {
        writer.Span<long>().WriteInt64BigEndian(value);
        return writer;
    }

    public static IBufferWriter<byte> WriteSingleBigEndian(this IBufferWriter<byte> writer, float value)
    {
        writer.Span<float>().WriteSingleBigEndian(value);
        return writer;
    }

    public static IBufferWriter<byte> WriteUInt16BigEndian(this IBufferWriter<byte> writer, ushort value)
    {
        writer.Span<ushort>().WriteUInt16BigEndian(value);
        return writer;
    }

    public static IBufferWriter<byte> WriteUInt32BigEndian(this IBufferWriter<byte> writer, uint value)
    {
        writer.Span<uint>().WriteUInt32BigEndian(value);
        return writer;
    }

    public static IBufferWriter<byte> WriteUInt64BigEndian(this IBufferWriter<byte> writer, ulong value)
    {
        writer.Span<ulong>().WriteUInt64BigEndian(value);
        return writer;
    }
}
