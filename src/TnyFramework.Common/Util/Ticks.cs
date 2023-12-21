// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace TnyFramework.Common.Util
{

    public static class Ticks
    {
        private const long TICKS_PER_MILLISECOND = 10000;
        private const long TICKS_PER_SECOND = TICKS_PER_MILLISECOND * 1000;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Tick() => Stopwatch.GetTimestamp();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long ToTick(TimeSpan span) => (long) (span.TotalMilliseconds * Stopwatch.Frequency / 1000);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long ToTick(long milliseconds) => milliseconds * Stopwatch.Frequency / 1000;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TimeSpan ToTimeSpan(long tick) => TimeSpan.FromMilliseconds((double) tick / Stopwatch.Frequency * 1000);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long ToMilliseconds(long tick) => (long) ((double) tick / Stopwatch.Frequency * 1000);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long ToIntervalMilliseconds(long startTick, long endTick) =>
            ToMilliseconds(endTick - startTick);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long ToIntervalMillisecondsUntilNow(long startTick) =>
            ToMilliseconds(Tick() - startTick);
    }

}
