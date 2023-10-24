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

namespace TnyFramework.Net.DotNetty.Common
{

    public static partial class ByteBufferUtils
    {
        /// <summary>
        /// 从字节数组中读取32位固定长度
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <param name="index">起始索引</param>
        /// <param name="value">读取的结果</param>
        /// <returns>读取的长度</returns>
        public static int ReadFixed32(byte[] bytes, int index, out int value)
        {
            value = bytes[index++] | bytes[index++] << 8 | bytes[index++] << 16 | bytes[index] << 24;
            return 4;
        }

        /// <summary>
        /// 从Stream读取32位固定长度
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="value"></param>
        public static void ReadFixed32(Stream stream, out int value)
        {
            value = stream.ReadByte() | stream.ReadByte() << 8 | stream.ReadByte() << 16 | stream.ReadByte() << 24;
        }

        /// <summary>
        /// 从IByteBuffer读取32位固定长度
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="value"></param>
        public static void ReadFixed32(IByteBuffer buffer, out int value)
        {
            value = buffer.ReadByte() | buffer.ReadByte() << 8 | buffer.ReadByte() << 16 | buffer.ReadByte() << 24;
        }

        /// <summary>
        /// 从字节数组中读取32位固定长度
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <param name="index">起始索引</param>
        /// <param name="value">读取的结果</param>
        /// <returns>读取的长度</returns>
        public static int ReadFixed32(byte[] bytes, int index, out float value)
        {
            var readed = ReadFixed32(bytes, index, out int intValue);
            value = Int2Float(intValue);
            return readed;
        }

        /// <summary>
        /// 从Stream读取32位固定长度
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="value"></param>
        public static void ReadFixed32(Stream stream, out float value)
        {
            ReadFixed32(stream, out int intValue);
            value = Int2Float(intValue);
        }

        /// <summary>
        /// 从IByteBuffer读取32位固定长度
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="value"></param>
        public static void ReadFixed32(IByteBuffer buffer, out float value)
        {
            ReadFixed32(buffer, out int intValue);
            value = Int2Float(intValue);
        }

        /// <summary>
        /// 从字节数组中读取64位固定长度
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <param name="index">起始索引</param>
        /// <param name="value">读取的结果</param>
        /// <returns>读取的长度</returns>
        public static int ReadFixed64(byte[] bytes, int index, out long value)
        {
            value = bytes[index++]
                    | ((long) bytes[index++] << 8)
                    | ((long) bytes[index++] << 16)
                    | ((long) bytes[index++] << 24)
                    | ((long) bytes[index++] << 32)
                    | ((long) bytes[index++] << 40)
                    | ((long) bytes[index++] << 48)
                    | ((long) bytes[index] << 56);
            return 8;
        }

        /// <summary>
        /// 从Stream读取64位固定长度
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="value"></param>
        public static void ReadFixed64(Stream stream, out long value)
        {
            value = (long) stream.ReadByte()
                    | ((long) stream.ReadByte() << 8)
                    | ((long) stream.ReadByte() << 16)
                    | ((long) stream.ReadByte() << 24)
                    | ((long) stream.ReadByte() << 32)
                    | ((long) stream.ReadByte() << 40)
                    | ((long) stream.ReadByte() << 48)
                    | ((long) stream.ReadByte() << 56);
        }

        /// <summary>
        /// 从IByteBuffer读取64位固定长度
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="value"></param>
        public static void ReadFixed64(IByteBuffer buffer, out long value)
        {
            value = (long) buffer.ReadByte()
                    | ((long) buffer.ReadByte() << 8)
                    | ((long) buffer.ReadByte() << 16)
                    | ((long) buffer.ReadByte() << 24)
                    | ((long) buffer.ReadByte() << 32)
                    | ((long) buffer.ReadByte() << 40)
                    | ((long) buffer.ReadByte() << 48)
                    | ((long) buffer.ReadByte() << 56);
        }

        /// <summary>
        /// 从字节数组中读取64位固定长度
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <param name="index">起始索引</param>
        /// <param name="value">读取的结果</param>
        /// <returns>读取的长度</returns>
        public static int ReadFixed64(byte[] bytes, int index, out double value)
        {
            var readed = ReadFixed64(bytes, index, out long longValue);
            value = Long2Double(longValue);
            return readed;
        }

        /// <summary>
        /// 从Stream读取64位固定长度
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="value"></param>
        public static void ReadFixed64(Stream stream, out double value)
        {
            ReadFixed64(stream, out long longValue);
            value = Long2Double(longValue);
        }

        /// <summary>
        /// 从IByteBuffer读取64位固定长度
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="value"></param>
        public static void ReadFixed64(IByteBuffer buffer, out double value)
        {
            ReadFixed64(buffer, out long longValue);
            value = Long2Double(longValue);
        }

        /// <summary>
        /// 从字节数组中读取动态长度
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <param name="index">起始索引</param>
        /// <param name="zigzag">是否采用zigzag压缩</param>
        /// <param name="value">读取的结果</param>
        /// <returns>读取的长度</returns>
        public static int ReadVariant(byte[] bytes, int index, bool zigzag, out int value)
        {
            var offset = index;
            var shift = 0;
            var result = 0;
            var time = 0;
            while (time < 10)
            {
                var b = bytes[offset++];
                result |= (b & 0x7F) << shift;
                if (shift < 32 && (b & 0x80) == 0)
                {
                    value = zigzag ? Zag(result) : result;
                    return offset - index;
                }
                shift += 7;
                time++;
            }
            throw new ArgumentException("error varint!!");
        }

        public static int ReadVariant(byte[] bytes, int index, out int value)
        {
            return ReadVariant(bytes, index, false, out value);
        }

        /// <summary>
        /// 从IByteBuffer中读取动态长度
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="zigzag"></param>
        /// <param name="value"></param>
        public static void ReadVariant(IByteBuffer buffer, bool zigzag, out int value)
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
                    value = zigzag ? Zag(result) : result;
                    return;
                }
                shift += 7;
                time++;
            }
            throw new ArgumentException("error varint!!");
        }

        public static void ReadVariant(IByteBuffer buffer, out int value)
        {
            ReadVariant(buffer, false, out value);
        }

        /// <summary>
        /// 从字节数组中读取动态长度
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <param name="index">起始索引</param>
        /// <param name="zigzag">是否采用zigzag压缩</param>
        /// <param name="value">读取的结果</param>
        /// <returns>读取的长度</returns>
        public static int ReadVariant(byte[] bytes, int index, bool zigzag, out bool value)
        {
            var readed = ReadVariant(bytes, index, zigzag, out int intValue);
            value = Convert.ToBoolean(intValue);
            return readed;
        }

        public static int ReadVariant(byte[] bytes, int index, out bool value)
        {
            var readed = ReadVariant(bytes, index, false, out int intValue);
            value = Convert.ToBoolean(intValue);
            return readed;
        }

        /// <summary>
        /// 从IByteBuffer中读取动态长度
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="zigzag"></param>
        /// <param name="value"></param>
        public static void ReadVariant(IByteBuffer buffer, bool zigzag, out bool value)
        {
            ReadVariant(buffer, zigzag, out int intValue);
            value = Convert.ToBoolean(intValue);
        }

        public static void ReadVariant(IByteBuffer buffer, out bool value)
        {
            ReadVariant(buffer, false, out int intValue);
            value = Convert.ToBoolean(intValue);
        }

        /// <summary>
        /// 从字节数组中读取动态长度
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <param name="index">起始索引</param>
        /// <param name="zigzag">是否采用zigzag压缩</param>
        /// <param name="value">读取的结果</param>
        /// <returns>读取的长度</returns>
        public static int ReadVariant(byte[] bytes, int index, bool zigzag, out byte value)
        {
            var readed = ReadVariant(bytes, index, zigzag, out int intValue);
            value = (byte) intValue;
            return readed;
        }

        public static int ReadVariant(byte[] bytes, int index, out byte value)
        {
            var readed = ReadVariant(bytes, index, false, out int intValue);
            value = (byte) intValue;
            return readed;
        }

        /// <summary>
        /// 从IByteBuffer中读取动态长度
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="zigzag"></param>
        /// <param name="value"></param>
        public static void ReadVariant(IByteBuffer buffer, bool zigzag, out byte value)
        {
            ReadVariant(buffer, zigzag, out int intValue);
            value = (byte) intValue;
        }

        public static void ReadVariant(IByteBuffer buffer, out byte value)
        {
            ReadVariant(buffer, false, out int intValue);
            value = (byte) intValue;
        }

        /// <summary>
        /// 从字节数组中读取动态长度
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <param name="index">起始索引</param>
        /// <param name="zigzag">是否采用zigzag压缩</param>
        /// <param name="value">读取的结果</param>
        /// <returns>读取的长度</returns>
        public static int ReadVariant(byte[] bytes, int index, bool zigzag, out sbyte value)
        {
            var readed = ReadVariant(bytes, index, zigzag, out int intValue);
            value = (sbyte) intValue;
            return readed;
        }

        public static int ReadVariant(byte[] bytes, int index, out sbyte value)
        {
            var readed = ReadVariant(bytes, index, false, out int intValue);
            value = (sbyte) intValue;
            return readed;
        }

        /// <summary>
        /// 从IByteBuffer中读取动态长度
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="zigzag"></param>
        /// <param name="value"></param>
        public static void ReadVariant(IByteBuffer buffer, bool zigzag, out sbyte value)
        {
            ReadVariant(buffer, zigzag, out int intValue);
            value = (sbyte) intValue;
        }

        public static void ReadVariant(IByteBuffer buffer, out sbyte value)
        {
            ReadVariant(buffer, false, out int intValue);
            value = (sbyte) intValue;
        }

        /// <summary>
        /// 从字节数组中读取动态长度
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <param name="index">起始索引</param>
        /// <param name="zigzag">是否采用zigzag压缩</param>
        /// <param name="value">读取的结果</param>
        /// <returns>读取的长度</returns>
        public static int ReadVariant(byte[] bytes, int index, bool zigzag, out float value)
        {
            var readed = ReadVariant(bytes, index, zigzag, out int intValue);
            value = Int2Float(intValue);
            return readed;
        }

        public static int ReadVariant(byte[] bytes, int index, out float value)
        {
            var readed = ReadVariant(bytes, index, false, out int intValue);
            value = Int2Float(intValue);
            return readed;
        }

        /// <summary>
        /// 从IByteBuffer中读取动态长度
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="zigzag"></param>
        /// <param name="value"></param>
        public static void ReadVariant(IByteBuffer buffer, bool zigzag, out float value)
        {
            ReadVariant(buffer, zigzag, out int intValue);
            value = Int2Float(intValue);
        }

        public static void ReadVariant(IByteBuffer buffer, out float value)
        {
            ReadVariant(buffer, false, out int intValue);
            value = Int2Float(intValue);
        }

        /// <summary>
        /// 从字节数组中读取动态长度
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <param name="index">起始索引</param>
        /// <param name="zigzag">是否采用zigzag压缩</param>
        /// <param name="value">读取的结果</param>
        /// <returns>读取的长度</returns>
        public static int ReadVariant(byte[] bytes, int index, bool zigzag, out long value)
        {
            var offset = index;
            var shift = 0;
            long result = 0;
            while (shift < 64)
            {
                var b = bytes[offset++];
                result |= (long) (b & 0x7F) << shift;
                if ((b & 0x80) == 0)
                {
                    value = zigzag ? Zag(result) : result;
                    return offset - index;
                }
                shift += 7;
            }
            throw new ArgumentException("error varint!!");
        }

        public static int ReadVariant(byte[] bytes, int index, out long value)
        {
            return ReadVariant(bytes, index, false, out value);
        }

        /// <summary>
        /// 从IByteBuffer中读取动态长度
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="zigzag"></param>
        /// <param name="value"></param>
        public static void ReadVariant(IByteBuffer buffer, bool zigzag, out long value)
        {
            var shift = 0;
            long result = 0;
            while (shift < 64)
            {
                var b = buffer.ReadByte();
                result |= (long) (b & 0x7F) << shift;
                if ((b & 0x80) == 0)
                {
                    value = zigzag ? Zag(result) : result;
                    return;
                }
                shift += 7;
            }
            throw new ArgumentException("error varint!!");
        }

        public static void ReadVariant(IByteBuffer buffer, out long value)
        {
            ReadVariant(buffer, false, out value);
        }

        /// <summary>
        /// 从字节数组中读取动态长度
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <param name="index">起始索引</param>
        /// <param name="zigzag">是否采用zigzag压缩</param>
        /// <param name="value">读取的结果</param>
        /// <returns>读取的长度</returns>
        public static int ReadVariant(byte[] bytes, int index, bool zigzag, out double value)
        {
            var readed = ReadVariant(bytes, index, zigzag, out long longValue);
            value = Long2Double(longValue);
            return readed;
        }

        public static int ReadVariant(byte[] bytes, int index, out double value)
        {
            var readed = ReadVariant(bytes, index, false, out long longValue);
            value = Long2Double(longValue);
            return readed;
        }

        /// <summary>
        /// 从IByteBuffer中读取动态长度
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="zigzag"></param>
        /// <param name="value"></param>
        public static void ReadVariant(IByteBuffer buffer, bool zigzag, out double value)
        {
            ReadVariant(buffer, zigzag, out long longValue);
            value = Long2Double(longValue);
        }

        public static void ReadVariant(IByteBuffer buffer, out double value)
        {
            ReadVariant(buffer, false, out long longValue);
            value = Long2Double(longValue);
        }

        /// <summary>
        /// 从字节数组中读取字符串
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <param name="index">起始索引</param>
        /// <param name="value">读取的结果</param>
        /// <returns>读取的长度</returns>
        public static int ReadString(byte[] bytes, int index, out string value)
        {
            var readed = ReadVariant(bytes, index, false, out int len);
            if (len == 0)
            {
                value = "";
                return readed;
            }
            value = ENCODING.GetString(bytes, readed + index, len);
            return readed + len;
        }

        /// <summary>
        /// 从IByteBuffer中读取动态长度
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="value"></param>
        public static void ReadString(IByteBuffer buffer, out string value)
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
        /// 从字节数组中读取字节数组
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <param name="index">起始索引</param>
        /// <param name="value">读取的结果</param>
        /// <returns>读取的长度</returns>
        public static int ReadBytes(byte[] bytes, int index, out byte[] value)
        {
            var readed = ReadVariant(bytes, index, false, out int len);
            if (len == 0)
            {
                value = new byte[0];
                return readed;
            }
            value = new byte[len];
            Buffer.BlockCopy(bytes, index + readed, value, 0, len);
            return readed + len;
        }

        /// <summary>
        /// 从IByteBuffer中读取动态长度
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="value"></param>
        public static void ReadBytes(IByteBuffer buffer, out byte[] value)
        {
            ReadVariant(buffer, false, out int len);
            if (len == 0)
            {
                value = new byte[0];
                return;
            }
            value = new byte[len];
            buffer.ReadBytes(value);
        }

        /// <summary>
        /// 从字节数组中读取字节数组
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <param name="index">起始索引</param>
        /// <param name="value">读取的结果</param>
        /// <returns>读取的长度</returns>
        public static int ReadBytes(byte[] bytes, int index, out ArraySegment<byte> value)
        {
            var readed = ReadVariant(bytes, index, false, out int len);
            if (len == 0)
            {
                value = new ArraySegment<byte>();
                return readed;
            }
            value = new ArraySegment<byte>(bytes, index + readed, len);
            return readed + len;
        }

        /// <summary>
        /// 从IByteBuffer中读取动态长度
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="value"></param>
        public static void ReadBytes(IByteBuffer buffer, out IByteBuffer value)
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

}
