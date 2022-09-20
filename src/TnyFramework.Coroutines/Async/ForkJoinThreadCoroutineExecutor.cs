// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Threading;

namespace TnyFramework.Coroutines.Async
{

    /// <summary>
    /// 线程池协程运行器
    /// </summary>
    public class ForkJoinThreadCoroutineExecutor : DedicatedThreadPoolCoroutineExecutor
    {
        private static volatile ICoroutineExecutor _INSTANCE;

        public static ICoroutineExecutor Default {
            get {
                if (_INSTANCE != null)
                    return _INSTANCE;
                lock (typeof(ForkJoinThreadCoroutineExecutor))
                {
                    if (_INSTANCE != null)
                        return _INSTANCE;
                    _INSTANCE = new ForkJoinThreadCoroutineExecutor("ForkJoinThreadCoroutineExecutor");
                }
                return _INSTANCE;
            }
        }

        protected ForkJoinThreadCoroutineExecutor(string name) : base(name)
        {
        }

        public ForkJoinThreadCoroutineExecutor(int threads, string name)
            : base(threads, name, null, 0)
        {
        }

        public ForkJoinThreadCoroutineExecutor(int threads, string name, Action<Exception> exceptionHandler = null, int threadMaxStackSize = 0)
            : base(threads, name, exceptionHandler, threadMaxStackSize)
        {
        }

        public ForkJoinThreadCoroutineExecutor(int threads, string name, TimeSpan? deadlockTimeout = null,
            Action<Exception> exceptionHandler = null, int threadMaxStackSize = 0) : base(threads, name, deadlockTimeout, exceptionHandler,
            threadMaxStackSize)
        {
        }

        public ForkJoinThreadCoroutineExecutor(int threads, string name, TimeSpan? deadlockTimeout = null,
            ApartmentState state = ApartmentState.MTA, Action<Exception> exceptionHandler = null, int threadMaxStackSize = 0) : base(
            threads, name, deadlockTimeout, state, exceptionHandler, threadMaxStackSize)
        {
        }
    }

}
