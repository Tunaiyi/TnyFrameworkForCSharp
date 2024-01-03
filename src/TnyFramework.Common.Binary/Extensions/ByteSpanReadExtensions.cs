// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;

namespace TnyFramework.Common.Binary.Extensions
{

    public static partial class ByteSpanReadExtensions
    {
        public static Span<byte> ReadByte(this Span<byte> destination, out byte value)
        {
            value = destination[0];
            return destination.Slice(sizeof(byte));
        }

        public static Span<byte> ReadByte(this Span<byte> destination, out sbyte value)
        {
            value = (sbyte) destination[0];
            return destination.Slice(sizeof(sbyte));
        }

        public static Span<byte> ReadBytes(this Span<byte> destination, out byte[] value, int length)
        {
            value = new byte[length];
            destination.CopyTo(value);
            // value.AsSpan(offset, length).CopyTo(destination);
            return destination.Slice(length);
        }

        public static Span<byte> ReadBytes(this Span<byte> destination, out byte[] value)
        {
            value = new byte[destination.Length];
            destination.CopyTo(value);
            return destination.Slice(destination.Length);
        }

        private static unsafe int SingleToInt32Bits(float value) => *(int*) &value;

        private static unsafe float Int32BitsToSingle(int value) => *(float*) &value;
    }

}
