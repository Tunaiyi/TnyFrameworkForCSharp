// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TnyFramework.Common.Logger;
using TnyFramework.Coroutines.ThreadPools;

namespace TnyFramework.Coroutines.TaskSchedulers
{

    public class DedicatedTaskScheduler : TaskScheduler
    {
        private static readonly ILogger LOGGER = LogFactory.Logger<DedicatedTaskScheduler>();

        private static readonly Action<Exception> DEFAULT_EXCEPTION_HANDLER = exception => {
            LOGGER.LogWarning(exception, $"CoroutineExecutor[{Thread.CurrentThread.Name}] execute exception");
        };

        private readonly DedicatedThreadPool threadPool;

        protected DedicatedTaskScheduler(string? name)
            : this(Environment.ProcessorCount, name, null, ApartmentState.Unknown, DEFAULT_EXCEPTION_HANDLER)
        {

        }

        public DedicatedTaskScheduler(int threads, string? name, Action<Exception>? exceptionHandler = null!,
            int threadMaxStackSize = 0)
            : this(threads, name, null, ApartmentState.Unknown, exceptionHandler, threadMaxStackSize)
        {

        }

        public DedicatedTaskScheduler(int threads, string? name, TimeSpan? deadlockTimeout = null,
            Action<Exception>? exceptionHandler = null!, int threadMaxStackSize = 0)
            : this(threads, name, deadlockTimeout, ApartmentState.Unknown, exceptionHandler, threadMaxStackSize)
        {

        }

        public DedicatedTaskScheduler(int threads, string? name, TimeSpan? deadlockTimeout = null,
            ApartmentState state = ApartmentState.Unknown, Action<Exception>? exceptionHandler = null, int threadMaxStackSize = 0)
        {
            var setting = new DedicatedThreadPoolSettings(threads, name, deadlockTimeout, state, exceptionHandler, threadMaxStackSize);
            threadPool = new DedicatedThreadPool(setting);
        }

        protected override IEnumerable<Task> GetScheduledTasks()
        {
            return Array.Empty<Task>();
        }

        protected override void QueueTask(Task task)
        {
            threadPool.QueueUserWorkItem(() => TryExecuteTask(task));
        }

        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            return TryExecuteTask(task);
        }
    }

}
