// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System.Collections.Generic;

namespace TnyFramework.Coroutines.Test
{

    public class SingleTest
    {
        public static readonly List<SingleTest> SINGLE_TESTS = new();

        private static readonly SingleTest INSTANCE; // 一次

        static SingleTest()
        {
            INSTANCE = new();
        }

        SingleTest()
        {
            SINGLE_TESTS.Add(this);
        }

        public static SingleTest Instance => INSTANCE;
    }

}
