// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Text;

namespace TnyFramework.Net.DotNetty.Common
{

    public static partial class ByteBufferUtils
    {
        private static readonly UTF8Encoding ENCODING = new UTF8Encoding();
        private const long INT64_MSB = 1L << 63;
        private const int INT32_MSB = 1 << 31;

        private const ulong MAX_LONG7_BIT = 0xffffffffffffffffL << 7;
        private const ulong MAX_LONG14_BIT = 0xffffffffffffffffL << 14;
        private const ulong MAX_LONG21_BIT = 0xffffffffffffffffL << 21;
        private const ulong MAX_LONG28_BIT = 0xffffffffffffffffL << 28;
        private const ulong MAX_LONG35_BIT = 0xffffffffffffffffL << 35;
        private const ulong MAX_LONG42_BIT = 0xffffffffffffffffL << 42;
        private const ulong MAX_LONG49_BIT = 0xffffffffffffffffL << 49;
        private const ulong MAX_LONG56_BIT = 0xffffffffffffffffL << 56;
        private const ulong MAX_LONG63_BIT = 0xffffffffffffffffL << 63;

        private const uint MAX_INT7_BIT = 0xffffffff << 7;
        private const uint MAX_INT14_BIT = 0xffffffff << 14;
        private const uint MAX_INT21_BIT = 0xffffffff << 21;
        private const uint MAX_INT28_BIT = 0xffffffff << 28;

        /// <summary>
        /// zig压缩
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static int Zig(int value)
        {
            return (value << 1) ^ (value >> 31);
        }

        /// <summary>
        /// zig压缩
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static long Zig(long value)
        {
            return (value << 1) ^ (value >> 63);
        }

        /// <summary>
        /// zag解压
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static int Zag(int value)
        {
            return (-(value & 0x01)) ^ ((value >> 1) & ~INT32_MSB);
        }

        /// <summary>
        /// zag解压
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static long Zag(long value)
        {
            return (-(value & 0x01L)) ^ ((value >> 1) & ~INT64_MSB);
        }

        /// <summary>
        /// 计算VarInt32的长度
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int ComputeVarInt32Len(int value)
        {
            return ComputeVarInt32Len((uint) value);
        }

        private static int ComputeVarInt32Len(uint value)
        {
            if ((value & MAX_INT7_BIT) == 0)
                return 1;
            if ((value & MAX_INT14_BIT) == 0)
                return 2;
            if ((value & MAX_INT21_BIT) == 0)
                return 3;
            if ((value & MAX_INT28_BIT) == 0)
                return 4;
            return 5;
        }

        /// <summary>
        /// 计算VarInt64的长度
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int ComputeVarInt64Len(long value)
        {
            return ComputeVarInt64Len((ulong) value);
        }

        private static int ComputeVarInt64Len(ulong value)
        {
            if ((value & MAX_LONG7_BIT) == 0)
                return 1;
            if ((value & MAX_LONG14_BIT) == 0)
                return 2;
            if ((value & MAX_LONG21_BIT) == 0)
                return 3;
            if ((value & MAX_LONG28_BIT) == 0)
                return 4;
            if ((value & MAX_LONG35_BIT) == 0)
                return 5;
            if ((value & MAX_LONG42_BIT) == 0)
                return 6;
            if ((value & MAX_LONG49_BIT) == 0)
                return 7;
            if ((value & MAX_LONG56_BIT) == 0)
                return 8;
            if ((value & MAX_LONG63_BIT) == 0)
                return 9;
            return 10;
        }

        /// <summary>
        /// 验证是否有足够空间
        /// </summary>
        /// <param name="required"></param>
        /// <param name="bytes"></param>
        /// <param name="index"></param>
        private static void VerifySpace(int required, byte[] bytes, int index)
        {
            if (bytes.Length - index < required)
            {
                throw new ArgumentOutOfRangeException(nameof(bytes), "没有足够的空间!!");
            }
        }

        private static long Double2Long(double value)
        {
            return BitConverter.ToInt64(BitConverter.GetBytes(value), 0);
        }

        private static int Float2Int(float value)
        {
            return BitConverter.ToInt32(BitConverter.GetBytes(value), 0);
        }

        private static double Long2Double(long value)
        {
            return BitConverter.ToDouble(BitConverter.GetBytes(value), 0);
        }

        private static float Int2Float(int value)
        {
            return BitConverter.ToSingle(BitConverter.GetBytes(value), 0);
        }
    }

}
