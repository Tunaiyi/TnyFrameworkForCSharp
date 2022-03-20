using System;
using System.Threading;
namespace TnyFramework.Net.Transport
{
    public interface IIdGenerator
    {
        long Generate();
    }

    public class AutoIncrementIdGenerator : IIdGenerator
    {
        private static readonly int PROCESSORS_SIZE = Environment.ProcessorCount;

        private volatile long[] idGenerators;

        private readonly int bitSize;
        private readonly int concurrentLevel;


        private static int BitCount(int n)
        {
            var count = 0;
            while (n != 0)
            {
                count++;
                n &= (n - 1);
            }
            return count;
        }


        public AutoIncrementIdGenerator() : this(PROCESSORS_SIZE)
        {

        }


        public AutoIncrementIdGenerator(int concurrentLevel)
        {
            this.concurrentLevel = concurrentLevel;
            idGenerators = new long[concurrentLevel];
            bitSize = BitCount(concurrentLevel);
        }


        public long Generate()
        {
            long threadId = Math.Abs(Thread.CurrentThread.ManagedThreadId);
            var index = threadId % concurrentLevel;
            var id = Interlocked.Increment(ref idGenerators[index]) << bitSize | index;
            return id;
        }
    }
}
