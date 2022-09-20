// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using TnyFramework.Namespace.Algorithm;

namespace TnyFramework.Namespace.Sharding
{

    public static class HashAlgorithmHasher
    {
        public const long UNLIMITED_SLOT_SIZE = -1;

        public static HashAlgorithmHasher<TValue> Hasher<TValue>(IHashAlgorithm algorithm)
        {
            return Hasher<TValue>(o => o.ToString(), algorithm);
        }

        public static HashAlgorithmHasher<TValue> Hasher<TValue>(long maxSlots, IHashAlgorithm algorithm)
        {
            return Hasher<TValue>(o => o.ToString(), maxSlots, algorithm);
        }

        public static HashAlgorithmHasher<TValue> Hasher<TValue>(Func<TValue, string> toKey, IHashAlgorithm algorithm)
        {
            return Hasher(toKey, UNLIMITED_SLOT_SIZE, algorithm);
        }

        public static HashAlgorithmHasher<TValue> Hasher<TValue>(Func<TValue, string> toKey, long maxSlots, IHashAlgorithm algorithm)
        {
            return new HashAlgorithmHasher<TValue>(toKey, algorithm, maxSlots);
        }
    }

    public class HashAlgorithmHasher<TValue> : BaseHasher<TValue>
    {
        private readonly IHashAlgorithm algorithm;

        private readonly long maxSlots;

        private readonly Func<TValue, string> toKey;

        public HashAlgorithmHasher(Func<TValue, string> toKey, IHashAlgorithm algorithm, long maxSlots)
        {
            this.algorithm = algorithm;
            this.maxSlots = maxSlots;
            this.toKey = toKey;
        }

        public override long Hash(TValue value, int seed)
        {
            var hashCode = algorithm.Hash(toKey.Invoke(value), seed);
            if (maxSlots < 0)
            {
                return hashCode;
            }
            return hashCode % maxSlots;
        }

        public long Max => maxSlots > 0 ? maxSlots : algorithm.Max;
    }

}
