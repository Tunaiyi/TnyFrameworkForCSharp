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
                    "MemoryAllotCounter 本次 分配 {} bytes 使用 {} bytes, 平均分配 {} bytes, 平均误差 {} bytes, 总分配 {} 次, 总分配 {} bytes, 最大 {} bytes, 最小 {} bytes",
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
