// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Threading;
using System.Threading.Tasks;
using TnyFramework.Coroutines.TaskSchedulers;
using TnyFramework.Coroutines.ThreadPools;

namespace TnyFramework.Coroutines.Executors
{

    /// <summary>
    /// 线程池协程运行器
    /// </summary>
    public class WorkStealingThreadPoolCoroutineExecutor : AbstractCoroutineExecutor
    {
        private readonly WorkStealingTaskScheduler threadPool;

        protected WorkStealingThreadPoolCoroutineExecutor(string name)
            : this(Environment.ProcessorCount, name)
        {
        }

        public WorkStealingThreadPoolCoroutineExecutor(int threads, IThreadFactory? threadFactory = null)
        {
            threadPool = new WorkStealingTaskScheduler(threads, threadFactory: threadFactory);
        }

        public WorkStealingThreadPoolCoroutineExecutor(int threads, string name = "", IThreadFactory? threadFactory = null)
        {
            threadPool = new WorkStealingTaskScheduler(threads, name, threadFactory);
        }

        public override void Summit(Action action)
        {
            Task.Factory.StartNew(action, CancellationToken.None, TaskCreationOptions.None, threadPool);
        }

    }

}
