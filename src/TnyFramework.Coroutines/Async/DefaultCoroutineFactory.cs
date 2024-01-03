// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System.Threading;
using System.Threading.Tasks;

namespace TnyFramework.Coroutines.Async
{

    /// <summary>
    /// 默认协程工厂
    /// </summary>
    public class DefaultCoroutineFactory : ICoroutineFactory
    {
        private int index;

        private readonly TaskScheduler executor;

        public static ICoroutineFactory Default { get; } = new DefaultCoroutineFactory("DefaultCoroutineFactory");

        /// <summary>
        /// 工厂名(作为协程名前缀)
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="executor">执行器</param>
        public DefaultCoroutineFactory(string name, TaskScheduler? executor = null)
        {
            Name = string.IsNullOrEmpty(name) ? "CoroutineFactory" : name;
            this.executor = executor ?? TaskScheduler.Default;
        }

        private string Name { get; }

        public Coroutine Create()
        {
            var currentIndex = Interlocked.Increment(ref index);
            return new DefaultCoroutine(executor, $"{Name}-{currentIndex}");
        }

        public Coroutine Create(string name)
        {
            var currentIndex = Interlocked.Increment(ref index);
            return new DefaultCoroutine(executor, $"{name}-{currentIndex}");
        }
    }

}
