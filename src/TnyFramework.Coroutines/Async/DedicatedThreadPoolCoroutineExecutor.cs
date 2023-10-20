// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Threading;
using Microsoft.Extensions.Logging;
using TnyFramework.Common.Logger;
using TnyFramework.Coroutines.ThreadPools;

namespace TnyFramework.Coroutines.Async
{

    /// <summary>
    /// 线程池协程运行器
    /// </summary>
    public class DedicatedThreadPoolCoroutineExecutor : ICoroutineExecutor
    {
        private static readonly ILogger LOGGER = LogFactory.Logger<DedicatedThreadPoolCoroutineExecutor>();

        private static readonly Action<Exception> DEFAULT_EXCEPTION_HANDLER = exception => {
            LOGGER.LogWarning(exception, $"CoroutineExecutor[{Thread.CurrentThread.Name}] execute exception");
        };

        private readonly DedicatedThreadPool threadPool;

        protected DedicatedThreadPoolCoroutineExecutor(string? name)
            : this(Environment.ProcessorCount, name, null, ApartmentState.Unknown, DEFAULT_EXCEPTION_HANDLER)
        {

        }

        public DedicatedThreadPoolCoroutineExecutor(int threads, string? name, Action<Exception>? exceptionHandler = null!,
            int threadMaxStackSize = 0)
            : this(threads, name, null, ApartmentState.Unknown, exceptionHandler, threadMaxStackSize)
        {

        }

        public DedicatedThreadPoolCoroutineExecutor(int threads, string? name, TimeSpan? deadlockTimeout = null,
            Action<Exception>? exceptionHandler = null!, int threadMaxStackSize = 0)
            : this(threads, name, deadlockTimeout, ApartmentState.Unknown, exceptionHandler, threadMaxStackSize)
        {

        }

        public DedicatedThreadPoolCoroutineExecutor(int threads, string? name, TimeSpan? deadlockTimeout = null,
            ApartmentState state = ApartmentState.Unknown, Action<Exception>? exceptionHandler = null, int threadMaxStackSize = 0)
        {
            var setting = new DedicatedThreadPoolSettings(threads, name, deadlockTimeout, state, exceptionHandler, threadMaxStackSize);
            threadPool = new DedicatedThreadPool(setting);
        }

        public void Summit(Action action)
        {
            threadPool.QueueUserWorkItem(action);
        }

        public void Shutdown()
        {
            threadPool.Dispose();
            threadPool.WaitForThreadsExit();
        }
    }

}
