using System;
using System.Collections.Generic;
using System.Text;

namespace TnyFramework.Common.Utils
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
