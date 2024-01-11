// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using DotNetty.Buffers;
using TnyFramework.Common.Binary.Extensions;

namespace TnyFramework.Net.DotNetty.Common;

public static partial class ByteBufferVariantExtensions
{
    /// <summary>
    /// 从IByteBuffer读取32位固定长度
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="value"></param>
    public static void ReadFixed32(this IByteBuffer buffer, out int value)
    {
        value = buffer.ReadByte() | buffer.ReadByte() << 8 | buffer.ReadByte() << 16 | buffer.ReadByte() << 24;
    }

    /// <summary>
    /// 从IByteBuffer读取32位固定长度
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="value"></param>
    public static void ReadFixed32(this IByteBuffer buffer, out float value)
    {
        ReadFixed32(buffer, out int intValue);
        value = VariantExtensions.Int2Float(intValue);
    }

    /// <summary>
    /// 从IByteBuffer读取64位固定长度
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="value"></param>
    public static void ReadFixed64(this IByteBuffer buffer, out long value)
    {
        value = buffer.ReadByte()
                | ((long) buffer.ReadByte() << 8)
                | ((long) buffer.ReadByte() << 16)
                | ((long) buffer.ReadByte() << 24)
                | ((long) buffer.ReadByte() << 32)
                | ((long) buffer.ReadByte() << 40)
                | ((long) buffer.ReadByte() << 48)
                | ((long) buffer.ReadByte() << 56);
    }

    /// <summary>
    /// 从IByteBuffer读取64位固定长度
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="value"></param>
    public static void ReadFixed64(this IByteBuffer buffer, out double value)
    {
        ReadFixed64(buffer, out long longValue);
        value = VariantExtensions.Long2Double(longValue);
    }

    /// <summary>
    /// 从IByteBuffer中读取动态长度
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="zigzag"></param>
    /// <param name="value"></param>
    public static void ReadVariant(this IByteBuffer buffer, bool zigzag, out int value)
    {
        var shift = 0;
        var result = 0;
        var time = 0;
        while (time < 10)
        {
            var b = buffer.ReadByte();
            result |= (b & 0x7F) << shift;
            if (shift < 32 && (b & 0x80) == 0)
            {
                value = zigzag ? VariantExtensions.Zag(result) : result;
                return;
            }
            shift += 7;
            time++;
        }
        throw new ArgumentException("error varint!!");
    }

    public static void ReadVariant(this IByteBuffer buffer, out int value)
    {
        ReadVariant(buffer, false, out value);
    }

    /// <summary>
    /// 从IByteBuffer中读取动态长度
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="zigzag"></param>
    /// <param name="value"></param>
    public static void ReadVariant(this IByteBuffer buffer, bool zigzag, out bool value)
    {
        ReadVariant(buffer, zigzag, out int intValue);
        value = Convert.ToBoolean(intValue);
    }

    public static void ReadVariant(this IByteBuffer buffer, out bool value)
    {
        ReadVariant(buffer, false, out int intValue);
        value = Convert.ToBoolean(intValue);
    }

    /// <summary>
    /// 从IByteBuffer中读取动态长度
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="zigzag"></param>
    /// <param name="value"></param>
    public static void ReadVariant(this IByteBuffer buffer, bool zigzag, out byte value)
    {
        ReadVariant(buffer, zigzag, out int intValue);
        value = (byte) intValue;
    }

    public static void ReadVariant(this IByteBuffer buffer, out byte value)
    {
        ReadVariant(buffer, false, out int intValue);
        value = (byte) intValue;
    }

    /// <summary>
    /// 从IByteBuffer中读取动态长度
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="zigzag"></param>
    /// <param name="value"></param>
    public static void ReadVariant(this IByteBuffer buffer, bool zigzag, out sbyte value)
    {
        ReadVariant(buffer, zigzag, out int intValue);
        value = (sbyte) intValue;
    }

    public static void ReadVariant(this IByteBuffer buffer, out sbyte value)
    {
        ReadVariant(buffer, false, out int intValue);
        value = (sbyte) intValue;
    }

    /// <summary>
    /// 从IByteBuffer中读取动态长度
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="zigzag"></param>
    /// <param name="value"></param>
    public static void ReadVariant(this IByteBuffer buffer, bool zigzag, out float value)
    {
        ReadVariant(buffer, zigzag, out int intValue);
        value = VariantExtensions.Int2Float(intValue);
    }

    public static void ReadVariant(this IByteBuffer buffer, out float value)
    {
        ReadVariant(buffer, false, out int intValue);
        value = VariantExtensions.Int2Float(intValue);
    }

    /// <summary>
    /// 从IByteBuffer中读取动态长度
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="zigzag"></param>
    /// <param name="value"></param>
    public static void ReadVariant(this IByteBuffer buffer, bool zigzag, out long value)
    {
        var shift = 0;
        long result = 0;
        while (shift < 64)
        {
            var b = buffer.ReadByte();
            result |= (long) (b & 0x7F) << shift;
            if ((b & 0x80) == 0)
            {
                value = zigzag ? VariantExtensions.Zag(result) : result;
                return;
            }
            shift += 7;
        }
        throw new ArgumentException("error varint!!");
    }

    public static void ReadVariant(this IByteBuffer buffer, out long value)
    {
        ReadVariant(buffer, false, out value);
    }

    /// <summary>
    /// 从IByteBuffer中读取动态长度
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="zigzag"></param>
    /// <param name="value"></param>
    public static void ReadVariant(this IByteBuffer buffer, bool zigzag, out double value)
    {
        ReadVariant(buffer, zigzag, out long longValue);
        value = VariantExtensions.Long2Double(longValue);
    }

    public static void ReadVariant(this IByteBuffer buffer, out double value)
    {
        ReadVariant(buffer, false, out long longValue);
        value = VariantExtensions.Long2Double(longValue);
    }

    /// <summary>
    /// 从IByteBuffer中读取动态长度
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="value"></param>
    public static void ReadString(this IByteBuffer buffer, out string value)
    {
        ReadVariant(buffer, false, out int len);
        if (len == 0)
        {
            value = "";
            return;
        }
        value = buffer.ReadString(len, ENCODING);
    }

    /// <summary>
    /// 从IByteBuffer中读取动态长度
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="value"></param>
    public static void ReadBytes(this IByteBuffer buffer, out byte[] value)
    {
        ReadVariant(buffer, false, out int len);
        if (len == 0)
        {
            value = Array.Empty<byte>();
            return;
        }
        value = new byte[len];
        buffer.ReadBytes(value);
    }

    /// <summary>
    /// 从IByteBuffer中读取动态长度
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="value"></param>
    public static void ReadBytes(this IByteBuffer buffer, out IByteBuffer value)
    {
        ReadVariant(buffer, false, out int len);
        if (len == 0)
        {
            value = null!;
            return;
        } else
        {
            value = buffer.Allocator.HeapBuffer(len);
        }
        buffer.ReadBytes(value, len);
    }
}
