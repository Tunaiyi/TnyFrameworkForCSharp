using System;
using System.Data.HashFunction;
using System.Data.HashFunction.CityHash;
using System.Data.HashFunction.FarmHash;
using System.Data.HashFunction.MetroHash;
using System.Data.HashFunction.MurmurHash;
using System.Data.HashFunction.xxHash;

namespace TnyFramework.Namespace.Algorithm.HashFunction
{

    public class HashFunctionHashAlgorithm : BaseHashAlgorithm
    {
        public static readonly IHashAlgorithm MURMUR3_32 = Algorithm32(seed => MurmurHash3Factory.Instance.Create(new MurmurHash3Config {
            HashSizeInBits = 128,
            Seed = (uint) seed
        }), true);

        public static readonly IHashAlgorithm MURMUR3_64 = Algorithm64(seed => MurmurHash3Factory.Instance.Create(new MurmurHash3Config {
            HashSizeInBits = 32,
            Seed = (uint) seed
        }), true);

        public static readonly IHashAlgorithm XX_HASH_32 = Algorithm32(seed => xxHashFactory.Instance.Create(new xxHashConfig {
            HashSizeInBits = 128,
            Seed = (uint) seed
        }), true);

        public static readonly IHashAlgorithm XX_HASH_64 = Algorithm64(seed => xxHashFactory.Instance.Create(new xxHashConfig {
            HashSizeInBits = 64,
            Seed = (uint) seed
        }), true);

        public static readonly IHashAlgorithm CITY_HASH_32 = Algorithm32(_ => CityHashFactory.Instance.Create(new CityHashConfig {
            HashSizeInBits = 32
        }), false);

        public static readonly IHashAlgorithm CITY_HASH_64 = Algorithm64WithManualSeed(_ => CityHashFactory.Instance.Create(new CityHashConfig {
            HashSizeInBits = 64
        }));

        public static readonly IHashAlgorithm FARM_HASH_32 = Algorithm32(_ => FarmHashFingerprint64Factory.Instance.Create(), false);

        public static readonly IHashAlgorithm FARM_HASH_64 = Algorithm64WithManualSeed(_ => FarmHashFingerprint64Factory.Instance.Create());

        public static readonly IHashAlgorithm METRO_HASH_32 = Algorithm32(seed => MetroHash64Factory.Instance.Create(new MetroHashConfig {
            Seed = (uint) seed
        }), true);

        public static readonly IHashAlgorithm METRO_HASH_64 = Algorithm64(_ => MetroHash64Factory.Instance.Create(), true);

        private const long K2 = -7286425919675154353L;
        private const long K_MUL = -7070675565921424023L;

        private readonly Func<int, IHashFunction> hashFunction;

        private readonly bool manualHashSeed;

        public static IHashAlgorithm Algorithm32(Func<int, IHashFunction> hashFunction, bool enableSeed)
        {
            return new HashFunctionHashAlgorithm(true, enableSeed, hashFunction);
        }

        public static IHashAlgorithm Algorithm64(Func<int, IHashFunction> hashFunction, bool enableSeed, bool enableCitySeed = false)
        {
            return new HashFunctionHashAlgorithm(false, enableSeed, hashFunction);
        }

        public static IHashAlgorithm Algorithm64WithManualSeed(Func<int, IHashFunction> hashFunction)
        {
            return new HashFunctionHashAlgorithm(false, true, hashFunction, true);
        }

        private HashFunctionHashAlgorithm(bool bit32, bool enableSeed, Func<int, IHashFunction> hashFunction) : base(bit32, enableSeed)
        {
            this.hashFunction = hashFunction;
            manualHashSeed = false;
        }

        private HashFunctionHashAlgorithm(bool bit32, bool enableSeed, Func<int, IHashFunction> hashFunction, bool manualHashSeed) : base(bit32,
            enableSeed)
        {
            this.hashFunction = hashFunction;
            this.manualHashSeed = manualHashSeed;

        }

        protected override long CountHash(string key, int seed)
        {
            var function = hashFunction.Invoke(seed);
            var hashCode = function.ComputeHash(DEFAULT_ENCODING.GetBytes(key));
            var bytes = hashCode.Hash;
            var code = bytes.Length == sizeof(uint) ? BitConverter.ToUInt32(bytes, 0) : (long) BitConverter.ToUInt64(bytes, 0);
            if (EnableSeed && manualHashSeed)
            {
                code = ManualHashSeed(code, seed);
            }
            return code;
        }

        private static long ManualHashSeed(long hash, long seed)
        {
            return ManualHashLen16(hash - K2, seed);

        }

        private static long ManualHashLen16(long u, long v)
        {
            var a = HashShiftMix((u ^ v) * K_MUL);
            return HashShiftMix((v ^ a) * K_MUL) * K_MUL;
        }

        private static long HashShiftMix(long val)
        {
            var temp = (ulong) val;
            return (long) (temp ^ (temp >> 47));
        }
    }

}
