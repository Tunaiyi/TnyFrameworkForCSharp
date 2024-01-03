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
using System.Text;
#endif

namespace TnyFramework.Common.Binary.Extensions
{

    public static partial class VariantExtensions
    {
#if NET
        /// <summary>
        /// 从IByteBuffer读取32位固定长度
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="value"></param>
        public static void ReadFixed32(this ref SequenceReader<byte> reader, out int value)
        {
            value = reader.ReadByte() | reader.ReadByte() << 8 | reader.ReadByte() << 16 | reader.ReadByte() << 24;
        }

        /// <summary>
        /// 从IByteBuffer读取32位固定长度
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="value"></param>
        public static void ReadFixed32(this ref SequenceReader<byte> reader, out float value)
        {
            reader.ReadFixed32(out int intValue);
            value = Int2Float(intValue);
        }

        /// <summary>
        /// 从IByteBuffer读取64位固定长度
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="value"></param>
        public static void ReadFixed64(this ref SequenceReader<byte> reader, out long value)
        {
            value = reader.ReadByte()
                    | ((long) reader.ReadByte() << 8)
                    | ((long) reader.ReadByte() << 16)
                    | ((long) reader.ReadByte() << 24)
                    | ((long) reader.ReadByte() << 32)
                    | ((long) reader.ReadByte() << 40)
                    | ((long) reader.ReadByte() << 48)
                    | ((long) reader.ReadByte() << 56);
        }

        /// <summary>
        /// 从IByteBuffer读取64位固定长度
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="value"></param>
        public static void ReadFixed64(this ref SequenceReader<byte> reader, out double value)
        {
            reader.ReadFixed64(out long longValue);
            value = Long2Double(longValue);
        }

        /// <summary>
        /// 从IByteBuffer中读取动态长度
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="zigzag"></param>
        /// <param name="value"></param>
        public static void ReadVariant(this ref SequenceReader<byte> reader, bool zigzag, out int value)
        {
            var shift = 0;
            var result = 0;
            var time = 0;
            while (time < 10)
            {
                var b = reader.ReadByte();
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

        public static void ReadVariant(this ref SequenceReader<byte> reader, out int value)
        {
            reader.ReadVariant(false, out value);
        }

        /// <summary>
        /// 从IByteBuffer中读取动态长度
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="zigzag"></param>
        /// <param name="value"></param>
        public static void ReadVariant(this ref SequenceReader<byte> reader, bool zigzag, out bool value)
        {
            reader.ReadVariant(zigzag, out int intValue);
            value = Convert.ToBoolean(intValue);
        }

        public static void ReadVariant(this ref SequenceReader<byte> reader, out bool value)
        {
            reader.ReadVariant(false, out int intValue);
            value = Convert.ToBoolean(intValue);
        }

        /// <summary>
        /// 从IByteBuffer中读取动态长度
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="zigzag"></param>
        /// <param name="value"></param>
        public static void ReadVariant(this ref SequenceReader<byte> reader, bool zigzag, out byte value)
        {
            reader.ReadVariant(zigzag, out int intValue);
            value = (byte) intValue;
        }

        public static void ReadVariant(this ref SequenceReader<byte> reader, out byte value)
        {
            reader.ReadVariant(false, out int intValue);
            value = (byte) intValue;
        }

        /// <summary>
        /// 从IByteBuffer中读取动态长度
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="zigzag"></param>
        /// <param name="value"></param>
        public static void ReadVariant(this ref SequenceReader<byte> reader, bool zigzag, out sbyte value)
        {
            reader.ReadVariant(zigzag, out int intValue);
            value = (sbyte) intValue;
        }

        public static void ReadVariant(this ref SequenceReader<byte> reader, out sbyte value)
        {
            reader.ReadVariant(false, out int intValue);
            value = (sbyte) intValue;
        }

        /// <summary>
        /// 从IByteBuffer中读取动态长度
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="zigzag"></param>
        /// <param name="value"></param>
        public static void ReadVariant(this ref SequenceReader<byte> reader, bool zigzag, out float value)
        {
            reader.ReadVariant(zigzag, out int intValue);
            value = Int2Float(intValue);
        }

        public static void ReadVariant(this ref SequenceReader<byte> reader, out float value)
        {
            reader.ReadVariant(false, out int intValue);
            value = Int2Float(intValue);
        }

        /// <summary>
        /// 从IByteBuffer中读取动态长度
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="zigzag"></param>
        /// <param name="value"></param>
        public static void ReadVariant(this ref SequenceReader<byte> reader, bool zigzag, out long value)
        {
            var shift = 0;
            long result = 0;
            while (shift < 64)
            {
                var b = reader.ReadByte();
                result |= (long) (b & 0x7F) << shift;
                if ((b & 0x80) == 0)
                {
                    value = zigzag ? Zag(result) : result;
                    return;
                }
                shift += 7;
            }
            throw new ArgumentException("error var int!!");
        }

        public static void ReadVariant(this ref SequenceReader<byte> reader, out long value)
        {
            reader.ReadVariant(false, out value);
        }

        /// <summary>
        /// 从IByteBuffer中读取动态长度
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="zigzag"></param>
        /// <param name="value"></param>
        public static void ReadVariant(this ref SequenceReader<byte> reader, bool zigzag, out double value)
        {
            reader.ReadVariant(zigzag, out long longValue);
            value = Long2Double(longValue);
        }

        public static void ReadVariant(this ref SequenceReader<byte> reader, out double value)
        {
            reader.ReadVariant(false, out long longValue);
            value = Long2Double(longValue);
        }

        /// <summary>
        /// 从IByteBuffer中读取动态长度
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="value"></param>
        public static void ReadVariantString(this ref SequenceReader<byte> reader, out string value)
        {
            reader.ReadVariant(false, out int len);
            if (len == 0)
            {
                value = "";
                return;
            }
            value = reader.ReadString(len, ENCODING);
        }

        /// <summary>
        /// 从IByteBuffer中读取动态长度
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="value"></param>
        public static void ReadVariantBytes(this ref SequenceReader<byte> reader, byte[] value)
        {
            reader.ReadVariant(false, out int len);
            if (len == 0)
            {
                return;
            }
            reader.TryCopyTo(value);
        }

        /// <summary>
        /// 从IByteBuffer中读取动态长度
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="value"></param>
        /// <param name="start"></param>
        /// <param name="count"></param>
        public static void ReadVariantBytes(this ref SequenceReader<byte> reader, byte[] value, int start, int count)
        {
            reader.ReadVariant(false, out int len);
            if (len == 0)
            {
                return;
            }
            reader.TryCopyTo(value.AsSpan(start, count));
        }

        /// <summary>
        /// 从IByteBuffer中读取动态长度
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="value"></param>
        public static void ReadVariantBytes(this ref SequenceReader<byte> reader, out byte[] value)
        {
            reader.ReadVariant(false, out int len);
            if (len == 0)
            {
                value = Array.Empty<byte>();
                return;
            }
            value = new byte[len];
            reader.TryCopyTo(value);
        }

        /// <summary>
        /// 从IByteBuffer中读取动态长度
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="buffer"></param>
        public static int ReadVariantBytes(this ref SequenceReader<byte> reader, IBufferWriter<byte> buffer)
        {
            reader.ReadVariant(false, out int len);
            if (len == 0)
            {
                return len;
            }
            reader.ReadBytes(len, buffer);
            return len;
        }
#endif
    }

}
