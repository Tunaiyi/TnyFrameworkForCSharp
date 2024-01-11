// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.IO;
using DotNetty.Buffers;
using TnyFramework.Common.Binary.Extensions;

namespace TnyFramework.Net.DotNetty.Common;

public static partial class ByteBufferVariantExtensions
{
    /// <summary>
    /// 写32位固定长度到字节数组
    /// </summary>
    /// <param name="value"></param>
    /// <param name="bytes"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public static int WriteFixed32(this byte[] bytes, int value, int index)
    {
        VariantExtensions.VerifySpace(4, bytes, index);
        bytes[index] = (byte) value;
        bytes[index + 1] = (byte) (value >> 8);
        bytes[index + 2] = (byte) (value >> 16);
        bytes[index + 3] = (byte) (value >> 24);
        return 4;
    }

    /// <summary>
    /// 写32位固定长度到Stream
    /// </summary>
    /// <param name="value"></param>
    /// <param name="stream"></param>
    public static void WriteFixed32(this Stream stream, int value)
    {
        stream.WriteByte((byte) value);
        stream.WriteByte((byte) (value >> 8));
        stream.WriteByte((byte) (value >> 16));
        stream.WriteByte((byte) (value >> 24));
    }

    /// <summary>
    /// 写32位固定长度到IByteBuffer
    /// </summary>
    /// <param name="value"></param>
    /// <param name="buffer"></param>
    public static void WriteFixed32(this IByteBuffer buffer, int value)
    {
        buffer.WriteByte((byte) value);
        buffer.WriteByte((byte) (value >> 8));
        buffer.WriteByte((byte) (value >> 16));
        buffer.WriteByte((byte) (value >> 24));
    }

    /// <summary>
    /// 写32位固定长度到字节数组
    /// </summary>
    /// <param name="value"></param>
    /// <param name="bytes"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public static int WriteFixed32(this byte[] bytes, float value, int index)
    {
        return WriteFixed32(bytes, VariantExtensions.Float2Int(value), index);
    }

    /// <summary>
    /// 写32位固定长度到Stream
    /// </summary>
    /// <param name="value"></param>
    /// <param name="stream"></param>
    public static void WriteFixed32(this Stream stream, float value)
    {
        WriteFixed32(stream, VariantExtensions.Float2Int(value));
    }

    /// <summary>
    /// 写32位固定长度到IByteBuffer
    /// </summary>
    /// <param name="value"></param>
    /// <param name="buffer"></param>
    public static void WriteFixed32(this IByteBuffer buffer, float value)
    {
        WriteFixed32(buffer, VariantExtensions.Float2Int(value));
    }

    /// <summary>
    /// 写64位固定长度到字节数组
    /// </summary>
    /// <param name="value"></param>
    /// <param name="bytes"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public static int WriteFixed64(this byte[] bytes, long value, int index)
    {
        VariantExtensions.VerifySpace(8, bytes, index);
        bytes[index] = (byte) value;
        bytes[index + 1] = (byte) (value >> 8);
        bytes[index + 2] = (byte) (value >> 16);
        bytes[index + 3] = (byte) (value >> 24);
        bytes[index + 4] = (byte) (value >> 32);
        bytes[index + 5] = (byte) (value >> 40);
        bytes[index + 6] = (byte) (value >> 48);
        bytes[index + 7] = (byte) (value >> 56);
        return 8;
    }

    /// <summary>
    /// 写64位固定长度到Stream
    /// </summary>
    /// <param name="value"></param>
    /// <param name="stream"></param>
    public static void WriteFixed64(this Stream stream, long value)
    {
        stream.WriteByte((byte) value);
        stream.WriteByte((byte) (value >> 8));
        stream.WriteByte((byte) (value >> 16));
        stream.WriteByte((byte) (value >> 24));
        stream.WriteByte((byte) (value >> 32));
        stream.WriteByte((byte) (value >> 40));
        stream.WriteByte((byte) (value >> 48));
        stream.WriteByte((byte) (value >> 56));
    }

    /// <summary>
    /// 写64位固定长度到IByteBuffer
    /// </summary>
    /// <param name="value"></param>
    /// <param name="buffer"></param>
    public static void WriteFixed64(this IByteBuffer buffer, long value)
    {
        buffer.WriteByte((byte) value);
        buffer.WriteByte((byte) (value >> 8));
        buffer.WriteByte((byte) (value >> 16));
        buffer.WriteByte((byte) (value >> 24));
        buffer.WriteByte((byte) (value >> 32));
        buffer.WriteByte((byte) (value >> 40));
        buffer.WriteByte((byte) (value >> 48));
        buffer.WriteByte((byte) (value >> 56));
    }

    /// <summary>
    /// 写64位固定长度到字节数组
    /// </summary>
    /// <param name="value"></param>
    /// <param name="bytes"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public static int WriteFixed64(this byte[] bytes, double value, int index)
    {
        return WriteFixed64(bytes, VariantExtensions.Double2Long(value), index);
    }

    /// <summary>
    /// 写64位固定长度到Stream
    /// </summary>
    /// <param name="value"></param>
    /// <param name="stream"></param>
    public static void WriteFixed64(this Stream stream, double value)
    {
        WriteFixed64(stream, VariantExtensions.Double2Long(value));
    }

    /// <summary>
    /// 写64位固定长度到IByteBuffer
    /// </summary>
    /// <param name="value"></param>
    /// <param name="buffer"></param>
    public static void WriteFixed64(this IByteBuffer buffer, double value)
    {
        WriteFixed64(buffer, VariantExtensions.Double2Long(value));
    }

    /// <summary>
    /// 写动态长度到字节数组
    /// </summary>
    /// <param name="value"></param>
    /// <param name="bytes"></param>
    /// <param name="index"></param>
    /// <param name="zigzag"></param>
    /// <returns></returns>
    public static int WriteVariant(this byte[] bytes, int value, int index, bool zigzag = false)
    {
        if (zigzag)
        {
            value = VariantExtensions.Zig(value);
        }
        var length = VariantExtensions.ComputeVarInt32Len((uint) value);
        VariantExtensions.VerifySpace(length, bytes, index); // 验证空间
        byte b;
        do
        {
            b = (byte) (value & 0x7F);
            if ((value >>= 7) != 0)
            {
                bytes[index++] = (byte) (b | 0x80);
            } else
            {
                bytes[index] = b;
                break;
            }
        } while (true);
        return length;
    }

    /// <summary>
    /// 写动态长度到IByteBuffer
    /// </summary>
    /// <param name="value"></param>
    /// <param name="buffer"></param>
    /// <param name="zigzag"></param>
    public static void WriteVariant(this IByteBuffer buffer, int value, bool zigzag = false)
    {
        if (zigzag)
        {
            value = VariantExtensions.Zig(value);
        }
        var uValue = (uint) value;
        byte b;
        do
        {
            b = (byte) (uValue & 0x7F);
            if ((uValue >>= 7) != 0)
            {
                buffer.WriteByte((byte) (b | 0x80));
            } else
            {
                buffer.WriteByte(b);
                break;
            }
        } while (true);
    }

    public static int WriteVariant(this byte[] bytes, bool value, int index, bool zigzag = false)
    {
        return WriteVariant(bytes, Convert.ToInt32(value), index, zigzag);
    }

    public static void WriteVariant(this IByteBuffer buffer, bool value, bool zigzag = false)
    {
        WriteVariant(buffer, Convert.ToInt32(value), zigzag);
    }

    public static int WriteVariant(this byte[] bytes, byte value, int index, bool zigzag = false)
    {
        return WriteVariant(bytes, (int) value, index, zigzag);
    }

    public static void WriteVariant(this IByteBuffer buffer, byte value, bool zigzag = false)
    {
        WriteVariant(buffer, (int) value, zigzag);
    }

    public static int WriteVariant(this byte[] bytes, sbyte value, int index, bool zigzag = false)
    {
        return WriteVariant(bytes, (int) value, index, zigzag);
    }

    public static int WriteVariant(this byte[] bytes, float value, int index, bool zigzag = false)
    {
        return WriteVariant(bytes, VariantExtensions.Float2Int(value), index, zigzag);
    }

    public static void WriteVariant(this IByteBuffer buffer, float value, bool zigzag = false)
    {
        WriteVariant(buffer, VariantExtensions.Float2Int(value), zigzag);
    }

    public static int WriteVariant(this byte[] bytes, long value, int index, bool zigzag = false)
    {
        if (zigzag)
        {
            value = VariantExtensions.Zig(value);
        }
        var length = VariantExtensions.ComputeVarInt64Len((ulong) value);
        VariantExtensions.VerifySpace(length, bytes, index);
        byte b;
        do
        {
            b = (byte) (value & 0x7F);
            if ((value >>= 7) != 0)
            {
                bytes[index++] = (byte) (b | 0x80);
            } else
            {
                bytes[index] = b;
                break;
            }
        } while (true);
        return length;
    }

    public static int WriteVariant(this IByteBuffer buffer, long value, bool zigzag = false)
    {
        if (zigzag)
        {
            value = VariantExtensions.Zig(value);
        }
        var uValue = (ulong) value;
        var startIndex = buffer.WriterIndex;
        byte b;
        do
        {
            b = (byte) (uValue & 0x7F);
            if ((uValue >>= 7) != 0)
            {
                buffer.WriteByte((byte) (b | 0x80));
            } else
            {
                buffer.WriteByte(b);
                break;
            }
        } while (true);
        return buffer.WriterIndex - startIndex;
    }

    public static int WriteVariant(this byte[] bytes, double value, int index, bool zigzag = false)
    {
        return WriteVariant(bytes, VariantExtensions.Double2Long(value), index, zigzag);
    }

    public static int WriteVariant(this IByteBuffer buffer, double value, bool zigzag = false)
    {
        return buffer.WriteVariant(VariantExtensions.Double2Long(value), zigzag);
    }

    public static void WriteString(this IByteBuffer buffer, string value)
    {
        var len = value.Length;
        if (len == 0)
        {
            WriteVariant(buffer, 0);
            return;
        }
        // 计算字符串字节长度
        var bytesLength = ENCODING.GetByteCount(value);
        // 写入长度
        WriteVariant(buffer, bytesLength);
        // 写入字符串
        buffer.WriteString(value, ENCODING);
    }

    public static void WriteBytes(this byte[] source, int sourceIndex, int length, IByteBuffer buffer)
    {
        WriteVariant(buffer, length);
        buffer.WriteBytes(source, sourceIndex, length);
    }

    public static void WriteBytes(this ArraySegment<byte> source, IByteBuffer buffer)
    {
        if (source.Array != null) WriteBytes(source.Array, source.Offset, source.Count, buffer);
    }
}
