using System;

namespace TnyFramework.Namespace.Sharding
{

    public abstract class BaseHasher<TValue> : IHasher<TValue>
    {

        public long Hash(TValue value, int seed, long max)
        {
            var code = Math.Abs(Hash(value, seed));
            if (max > 0L)
            {
                return code % max;
            }
            return code;
        }

        public abstract long Hash(TValue value, int seed);
    }

}
