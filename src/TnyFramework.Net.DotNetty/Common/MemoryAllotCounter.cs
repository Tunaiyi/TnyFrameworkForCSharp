// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using Microsoft.Extensions.Logging;
using TnyFramework.Common.Logger;

namespace TnyFramework.Net.DotNetty.Common
{

    public class MemoryAllotCounter
    {
        private static readonly ILogger LOGGER = LogFactory.Logger<MemoryAllotCounter>();

        private int totalSize;

        private int count;

        private int deviationSize;

        private int maxSize;

        private int minSize = int.MaxValue;

        private long time = DateTimeOffset.Now.ToUnixTimeMilliseconds();

        private readonly int initSize;

        private readonly long interval;

        public MemoryAllotCounter() : this(2048, 10 * 60000L)
        {

        }

        public MemoryAllotCounter(int initSize, long interval)
        {
            this.initSize = initSize;
            this.interval = interval;
        }

        public void Recode(int allot, int useSize)
        {
            var now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            if (now > time)
            {
                Reset();
            }
            totalSize += useSize;
            if (useSize > maxSize)
            {
                maxSize = useSize;
            }
            if (useSize < minSize)
            {
                minSize = useSize;
            }
            deviationSize += Math.Max(useSize - allot, 0);
            count++;
            if (LOGGER.IsEnabled(LogLevel.Debug))
            {
                LOGGER.LogDebug(
                    "MemoryAllotCounter 本次 分配 {allot} bytes 使用 {use} bytes, 平均分配 {perAllotSize} bytes, 平均误差 {perDevSize} bytes, 总分配 {count} 次, 总分配 {total} bytes, 最大 {maxSize} bytes, 最小 {minSize} bytes",
                    allot, useSize, totalSize / count, deviationSize / count, count, totalSize, maxSize, minSize);
            }
        }

        public int Allot()
        {
            if (count == 0)
            {
                return initSize;
            }
            return Math.Max((totalSize / count) * 2, initSize) + (deviationSize / count) * 2;
        }

        private void Reset()
        {
            totalSize = 0;
            count = 0;
            maxSize = 0;
            minSize = int.MaxValue;
            time = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }
    }

}
