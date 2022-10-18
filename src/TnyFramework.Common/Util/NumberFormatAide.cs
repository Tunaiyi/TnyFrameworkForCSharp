// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Collections.Generic;
using System.Text;

namespace TnyFramework.Common.Util
{

    public static class NumberFormatAide
    {
        private static readonly int LONG_MAX_DIGITS = $"{long.MaxValue}".Length;

        private static readonly List<string> ZERO_FILL;

        static NumberFormatAide()
        {
            var builder = new StringBuilder();
            var zeroFill = new List<string>();
            for (var index = 0; index < LONG_MAX_DIGITS; index++)
            {
                if (index > 0)
                {
                    builder.Append(0);
                }
                zeroFill.Add(builder.ToString());
            }
            ZERO_FILL = zeroFill;
        }

        public static string AlignDigits(long hashCode, long maxCode)
        {

            var digits = (int) Math.Floor(Math.Log10(hashCode) + 1);
            var maxDigits = (int) Math.Floor(Math.Log10(hashCode) + 1);
            var lack = maxDigits - digits;
            if (lack == 0)
            {
                return hashCode.ToString();
            }
            return ZERO_FILL[lack] + hashCode;
        }
    }

}
