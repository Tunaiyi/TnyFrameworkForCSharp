// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using TnyFramework.Common.Exceptions;

namespace TnyFramework.Common.Util
{

    public class Random
    {
        private const long MULTIPLIER = 0x5DEECE66DL;
        private const long ADDEND = 0xBL;
        private const long MASK = (1L << 48) - 1;

        // IllegalArgumentException messages
        private long seed;

        public Random(long seed, bool scramble = false)
        {
            this.seed = scramble ? InitialScramble(seed) : seed;
        }

        public long Seed => seed;

        public int NextInt()
        {
            return Next(32);
        }

        private static long InitialScramble(long seed)
        {
            return (seed ^ MULTIPLIER) & MASK;
        }

        public int NextInt(int bound)
        {
            if (bound <= 0)
                throw new IllegalArgumentException("bound must be positive");
            if ((bound & -bound) == bound) // i.e., bound is a power of 2
                return (int) ((bound * (long) Next(31)) >> 31);
            int bits, val;
            do
            {
                bits = Next(31);
                val = bits % bound;
            } while (bits - val + (bound - 1) < 0);
            return val;
        }

        public long NextLong()
        {
            return ((long) Next(32) << 32) + Next(32);
        }

        private int Next(int bits)
        {
            var newSeed = (seed * MULTIPLIER + ADDEND) & MASK;
            seed = newSeed;
            return (int) ((ulong) newSeed >> (48 - bits));
        }
    }

}
