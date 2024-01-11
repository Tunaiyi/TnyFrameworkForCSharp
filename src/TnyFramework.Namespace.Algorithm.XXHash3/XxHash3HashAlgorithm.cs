// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;

namespace TnyFramework.Namespace.Algorithm.XXHash3;

public class XxHash3HashAlgorithm : BaseHashAlgorithm
{
    public static readonly IHashAlgorithm XXH3_HASH_32 = new XxHash3HashAlgorithm(true);

    public static readonly IHashAlgorithm XXH3_HASH_64 = new XxHash3HashAlgorithm();

    private XxHash3HashAlgorithm(bool bit32 = false) : base(bit32, true)
    {
    }

    protected override long CountHash(string key, int seed)
    {
        var span = new ReadOnlySpan<byte>(DEFAULT_ENCODING.GetBytes(key));
        return (long) XXHash3NET.XXHash3.Hash64(span, (ulong) seed);
    }
}
