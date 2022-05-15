using System.Collections.Generic;
using System.Reflection;
using TnyFramework.Common.Exceptions;

namespace TnyFramework.Net.Dispatcher
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
