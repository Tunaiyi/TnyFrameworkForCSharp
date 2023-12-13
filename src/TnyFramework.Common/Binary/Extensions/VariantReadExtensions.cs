using System;
using System.Buffers;
using System.IO;

namespace TnyFramework.Common.Binary.Extensions
{

    public static partial class VariantExtensions
    {
        /// <summary>
        /// 从字节数组中读取32位固定长度
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <param name="index">起始索引</param>
        /// <param name="value">读取的结果</param>
        /// <returns>读取的长度</returns>
        public static int ReadFixed32(this byte[] bytes, int index, out int value)
        {
            value = bytes[index++] | bytes[index++] << 8 | bytes[index++] << 16 | bytes[index] << 24;
            return 4;
        }

        /// <summary>
        /// 从Stream读取32位固定长度
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="value"></param>
        public static void ReadFixed32(this Stream stream, out int value)
        {
            value = stream.ReadByte() | stream.ReadByte() << 8 | stream.ReadByte() << 16 | stream.ReadByte() << 24;
        }

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
        /// 从字节数组中读取32位固定长度
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <param name="index">起始索引</param>
        /// <param name="value">读取的结果</param>
        /// <returns>读取的长度</returns>
        public static int ReadFixed32(this byte[] bytes, int index, out float value)
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
        public static void ReadFixed32(this Stream stream, out float value)
        {
            ReadFixed32(stream, out int intValue);
            value = Int2Float(intValue);
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
        /// 从字节数组中读取64位固定长度
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <param name="index">起始索引</param>
        /// <param name="value">读取的结果</param>
        /// <returns>读取的长度</returns>
        public static int ReadFixed64(this byte[] bytes, int index, out long value)
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
        public static void ReadFixed64(this Stream stream, out long value)
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
        /// 从字节数组中读取64位固定长度
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <param name="index">起始索引</param>
        /// <param name="value">读取的结果</param>
        /// <returns>读取的长度</returns>
        public static int ReadFixed64(this byte[] bytes, int index, out double value)
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
        public static void ReadFixed64(this Stream stream, out double value)
        {
            ReadFixed64(stream, out long longValue);
            value = Long2Double(longValue);
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
        /// 从字节数组中读取动态长度
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <param name="index">起始索引</param>
        /// <param name="zigzag">是否采用zigzag压缩</param>
        /// <param name="value">读取的结果</param>
        /// <returns>读取的长度</returns>
        public static int ReadVariant(this byte[] bytes, int index, bool zigzag, out int value)
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

        public static int ReadVariant(this byte[] bytes, int index, out int value)
        {
            return ReadVariant(bytes, index, false, out value);
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
        /// 从字节数组中读取动态长度
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <param name="index">起始索引</param>
        /// <param name="zigzag">是否采用zigzag压缩</param>
        /// <param name="value">读取的结果</param>
        /// <returns>读取的长度</returns>
        public static int ReadVariant(this byte[] bytes, int index, bool zigzag, out bool value)
        {
            var readed = ReadVariant(bytes, index, zigzag, out int intValue);
            value = Convert.ToBoolean(intValue);
            return readed;
        }

        public static int ReadVariant(this byte[] bytes, int index, out bool value)
        {
            var readed = ReadVariant(bytes, index, false, out int intValue);
            value = Convert.ToBoolean(intValue);
            return readed;
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
        /// 从字节数组中读取动态长度
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <param name="index">起始索引</param>
        /// <param name="zigzag">是否采用zigzag压缩</param>
        /// <param name="value">读取的结果</param>
        /// <returns>读取的长度</returns>
        public static int ReadVariant(this byte[] bytes, int index, bool zigzag, out byte value)
        {
            var readed = ReadVariant(bytes, index, zigzag, out int intValue);
            value = (byte) intValue;
            return readed;
        }

        public static int ReadVariant(this byte[] bytes, int index, out byte value)
        {
            var readed = ReadVariant(bytes, index, false, out int intValue);
            value = (byte) intValue;
            return readed;
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
        /// 从字节数组中读取动态长度
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <param name="index">起始索引</param>
        /// <param name="zigzag">是否采用zigzag压缩</param>
        /// <param name="value">读取的结果</param>
        /// <returns>读取的长度</returns>
        public static int ReadVariant(this byte[] bytes, int index, bool zigzag, out sbyte value)
        {
            var readed = ReadVariant(bytes, index, zigzag, out int intValue);
            value = (sbyte) intValue;
            return readed;
        }

        public static int ReadVariant(this byte[] bytes, int index, out sbyte value)
        {
            var readed = ReadVariant(bytes, index, false, out int intValue);
            value = (sbyte) intValue;
            return readed;
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
        /// 从字节数组中读取动态长度
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <param name="index">起始索引</param>
        /// <param name="zigzag">是否采用zigzag压缩</param>
        /// <param name="value">读取的结果</param>
        /// <returns>读取的长度</returns>
        public static int ReadVariant(this byte[] bytes, int index, bool zigzag, out float value)
        {
            var readed = ReadVariant(bytes, index, zigzag, out int intValue);
            value = Int2Float(intValue);
            return readed;
        }

        public static int ReadVariant(this byte[] bytes, int index, out float value)
        {
            var readed = ReadVariant(bytes, index, false, out int intValue);
            value = Int2Float(intValue);
            return readed;
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
        /// 从字节数组中读取动态长度
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <param name="index">起始索引</param>
        /// <param name="zigzag">是否采用zigzag压缩</param>
        /// <param name="value">读取的结果</param>
        /// <returns>读取的长度</returns>
        public static int ReadVariant(this byte[] bytes, int index, bool zigzag, out long value)
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

        public static int ReadVariant(this byte[] bytes, int index, out long value)
        {
            return ReadVariant(bytes, index, false, out value);
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
            throw new ArgumentException("error varint!!");
        }

        public static void ReadVariant(this ref SequenceReader<byte> reader, out long value)
        {
            reader.ReadVariant(false, out value);
        }

        /// <summary>
        /// 从字节数组中读取动态长度
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <param name="index">起始索引</param>
        /// <param name="zigzag">是否采用zigzag压缩</param>
        /// <param name="value">读取的结果</param>
        /// <returns>读取的长度</returns>
        public static int ReadVariant(this byte[] bytes, int index, bool zigzag, out double value)
        {
            var readed = ReadVariant(bytes, index, zigzag, out long longValue);
            value = Long2Double(longValue);
            return readed;
        }

        public static int ReadVariant(this byte[] bytes, int index, out double value)
        {
            var readed = ReadVariant(bytes, index, false, out long longValue);
            value = Long2Double(longValue);
            return readed;
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
        /// 从字节数组中读取字符串
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <param name="index">起始索引</param>
        /// <param name="value">读取的结果</param>
        /// <returns>读取的长度</returns>
        public static int ReadStringWithLength(this byte[] bytes, int index, out string value)
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
        /// <param name="reader"></param>
        /// <param name="value"></param>
        public static void ReadStringWithLength(this ref SequenceReader<byte> reader, out string value)
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
        /// 从字节数组中读取字节数组
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <param name="index">起始索引</param>
        /// <param name="value">读取的结果</param>
        /// <returns>读取的长度</returns>
        public static int ReadBytesWithLength(this byte[] bytes, int index, out byte[] value)
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
        /// <param name="reader"></param>
        /// <param name="value"></param>
        public static void ReadBytesWithLength(this ref SequenceReader<byte> reader, byte[] value)
        {
            reader.ReadVariant(false, out int len);
            if (len == 0)
            {
                value = Array.Empty<byte>();
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
        public static void ReadBytesWithLength(this ref SequenceReader<byte> reader, byte[] value, int start, int count)
        {
            reader.ReadVariant(false, out int len);
            if (len == 0)
            {
                value = Array.Empty<byte>();
                return;
            }
            reader.TryCopyTo(value.AsSpan(start, count));
        }

        /// <summary>
        /// 从IByteBuffer中读取动态长度
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="value"></param>
        public static void ReadBytesWithLength(this ref SequenceReader<byte> reader, out byte[] value)
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
        /// 从字节数组中读取字节数组
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <param name="index">起始索引</param>
        /// <param name="value">读取的结果</param>
        /// <returns>读取的长度</returns>
        public static int ReadBytesWithLength(this byte[] bytes, int index, out ArraySegment<byte> value)
        {
            var read = ReadVariant(bytes, index, false, out int len);
            if (len == 0)
            {
                value = new ArraySegment<byte>();
                return read;
            }
            value = new ArraySegment<byte>(bytes, index + read, len);
            return read + len;
        }

        /// <summary>
        /// 从IByteBuffer中读取动态长度
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="buffer"></param>
        public static int ReadBytesWithLength(this ref SequenceReader<byte> reader, IBufferWriter<byte> buffer)
        {
            reader.ReadVariant(false, out int len);
            if (len == 0)
            {
                return len;
            }
            reader.ReadBytes(len, buffer);
            return len;
        }
    }

}
