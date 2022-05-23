using System;

namespace TnyFramework.Namespace.Algorithm.XXHash3
{

    public class XxHash3HashAlgorithm : BaseHashAlgorithm
    {
        public static readonly IHashAlgorithm XXH3_HASH_32 = new XxHash3HashAlgorithm(true);

        public static readonly IHashAlgorithm XXH3_HASH_64 = new XxHash3HashAlgorithm();

        private XxHash3HashAlgorithm(bool bit32 = false) : base(bit32, true)
        {
        }

        protected override long CountHash(string key, int seed)
        {
            return (long) XXHash3NET.XXHash3.Hash64(new ReadOnlySpan<byte>(DEFAULT_ENCODING.GetBytes(key)), seed);
        }
    }

}
