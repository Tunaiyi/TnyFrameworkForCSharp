using System.Text;

namespace TnyFramework.Namespace.Algorithm
{

    public abstract class BaseHashAlgorithm : IHashAlgorithm
    {
        public static readonly Encoding DEFAULT_ENCODING = Encoding.UTF8;

        private const long MASH_32 = 0xFFFFFFFFL << 32;

        private const long BIT_32_MAX = (long) (0xFFFFFFFFFFFFFFFFL >> 32);

        protected bool EnableBit32 { get; }

        protected bool EnableSeed { get; }

        protected BaseHashAlgorithm(bool bit32, bool enableSeed)
        {
            EnableBit32 = bit32;
            EnableSeed = enableSeed;
        }

        public long Hash(string value, int seed)
        {
            if (!EnableSeed && seed != 0)
            {
                value += seed;
            }
            var code = CountHash(value, seed);
            if (!EnableBit32 || (code & MASH_32) == 0)
            {
                return code;
            }
            return (long) (ulong) code >> 32;
        }

        public long Max => EnableBit32 ? BIT_32_MAX : long.MaxValue;

        protected abstract long CountHash(string key, int seed);
    }

}
