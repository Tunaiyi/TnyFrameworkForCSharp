// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.IO;

namespace TnyFramework.Codec.ProtobufNet.TypeProtobuf
{

    public static partial class ByteUtils
    {
        /// <summary>
        /// 写32位固定长度到字节数组
        /// </summary>
        /// <param name="value"></param>
        /// <param name="bytes"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static int WriteFixed32(int value, byte[] bytes, int index)
        {
            VerifySpace(4, bytes, index);
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
        public static void WriteFixed32(int value, Stream stream)
        {
            stream.WriteByte((byte) value);
            stream.WriteByte((byte) (value >> 8));
            stream.WriteByte((byte) (value >> 16));
            stream.WriteByte((byte) (value >> 24));
        }

        /// <summary>
        /// 写32位固定长度到字节数组
        /// </summary>
        /// <param name="value"></param>
        /// <param name="bytes"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static int WriteFixed32(float value, byte[] bytes, int index)
        {
            return WriteFixed32(Float2Int(value), bytes, index);
        }

        /// <summary>
        /// 写32位固定长度到Stream
        /// </summary>
        /// <param name="value"></param>
        /// <param name="stream"></param>
        public static void WriteFixed32(float value, Stream stream)
        {
            WriteFixed32(Float2Int(value), stream);
        }

        /// <summary>
        /// 写64位固定长度到字节数组
        /// </summary>
        /// <param name="value"></param>
        /// <param name="bytes"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static int WriteFixed64(long value, byte[] bytes, int index)
        {
            VerifySpace(8, bytes, index);
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
        public static void WriteFixed64(long value, Stream stream)
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
        /// 写64位固定长度到字节数组
        /// </summary>
        /// <param name="value"></param>
        /// <param name="bytes"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static int WriteFixed64(double value, byte[] bytes, int index)
        {
            return WriteFixed64(Double2Long(value), bytes, index);
        }

        /// <summary>
        /// 写64位固定长度到Stream
        /// </summary>
        /// <param name="value"></param>
        /// <param name="stream"></param>
        public static void WriteFixed64(double value, Stream stream)
        {
            WriteFixed64(Double2Long(value), stream);
        }

        /// <summary>
        /// 写动态长度到字节数组
        /// </summary>
        /// <param name="value"></param>
        /// <param name="bytes"></param>
        /// <param name="index"></param>
        /// <param name="zigzag"></param>
        /// <returns></returns>
        public static int WriteVariant(int value, byte[] bytes, int index, bool zigzag = false)
        {
            if (zigzag)
            {
                value = Zig(value);
            }
            var length = ComputeVarInt32Len((uint) value);
            VerifySpace(length, bytes, index); // 验证空间
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

        public static int WriteVariant(bool value, byte[] bytes, int index, bool zigzag = false)
        {
            return WriteVariant(Convert.ToInt32(value), bytes, index, zigzag);
        }

        public static int WriteVariant(byte value, byte[] bytes, int index, bool zigzag = false)
        {
            return WriteVariant((int) value, bytes, index, zigzag);
        }

        public static int WriteVariant(sbyte value, byte[] bytes, int index, bool zigzag = false)
        {
            return WriteVariant((int) value, bytes, index, zigzag);
        }

        public static int WriteVariant(float value, byte[] bytes, int index, bool zigzag = false)
        {
            return WriteVariant(Float2Int(value), bytes, index, zigzag);
        }

        public static int WriteVariant(long value, byte[] bytes, int index, bool zigzag = false)
        {
            if (zigzag)
            {
                value = Zig(value);
            }
            var length = ComputeVarInt64Len((ulong) value);
            VerifySpace(length, bytes, index);
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

        public static int WriteVariant(double value, byte[] bytes, int index, bool zigzag = false)
        {
            return WriteVariant(Double2Long(value), bytes, index, zigzag);
        }

        public static int WriteString(string value, byte[] bytes, int index)
        {
            var len = value.Length;
            if (len == 0)
            {
                return WriteVariant(0, bytes, index);
            }
            var predicted = encoding.GetByteCount(value);
            var byteWrite = WriteVariant(predicted, bytes, index);
            index += byteWrite;
            VerifySpace(predicted, bytes, index); // 验证空间
            encoding.GetBytes(value, 0, len, bytes, index);
            byteWrite += predicted;
            return byteWrite;
        }

        public static int WriteBytes(byte[] source, int sourceIndex, int length, byte[] bytes, int index)
        {
            var byteWrite = WriteVariant(length, bytes, index);
            index += byteWrite;
            VerifySpace(length, bytes, index);
            Buffer.BlockCopy(source, sourceIndex, bytes, index, length);
            byteWrite += length;
            return byteWrite;
        }

        public static int WriteBytes(ArraySegment<byte> source, byte[] bytes, int index)
        {
            return WriteBytes(source.Array, source.Offset, source.Count, bytes, index);
        }
    }

}
