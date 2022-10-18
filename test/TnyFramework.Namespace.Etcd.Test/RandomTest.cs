// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using NUnit.Framework;
using Random = TnyFramework.Common.Util.Random;

namespace TnyFramework.Namespace.Etcd.Test
{

    public class RandomTest
    {
        [Test]
        public void TestRandom()
        {
            var random = new Random(199999L, true);
            for (var i = 0; i < 20; i++)
            {
                Console.WriteLine(random.NextInt(100));
            }
            var seed = random.Seed;
            random = new Random(seed);
            for (var i = 0; i < 20; i++)
            {
                Console.WriteLine(random.NextInt(100));
            }
        }
    }

}
