// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System.Collections.Generic;
using System.Reflection;
using TnyFramework.Common.Exceptions;

namespace TnyFramework.Net.Command.Dispatcher
{

    public class ParamIndexCreator
    {
        private readonly MethodInfo method;

        private readonly SortedSet<int> useIndexes = new SortedSet<int>();

        private int step;

        public ParamIndexCreator(MethodInfo method)
        {
            this.method = method;
        }

        public int Peek()
        {
            int index;
            do
            {
                index = step;
                step++;
            } while (!useIndexes.Add(index));
            return index;
        }

        public int Use(int index)
        {
            if (useIndexes.Add(index))
            {
                return index;
            }
            throw new IllegalArgumentException($"{method} 反复 {index} 参数出现重复");
        }
    }

}
