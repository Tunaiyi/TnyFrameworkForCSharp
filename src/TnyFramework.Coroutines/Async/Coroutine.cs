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

namespace TnyFramework.Coroutines.Async
{

    public abstract class Coroutine : CoroutineTaskScheduler, ICoroutine
    {
        private static readonly ThreadLocal<Coroutine?> CURRENT_COROUTINE = new();

        public static Coroutine? CurrentCoroutine => CURRENT_COROUTINE.Value;

        protected static void SetCurrentCoroutine(Coroutine? coroutine)
        {
            CURRENT_COROUTINE.Value = coroutine;
        }

        public abstract Task AsyncExec(AsyncHandle handle);

        public abstract Task<T> AsyncExec<T>(AsyncHandle<T> function);

        internal abstract Task AsyncExec(ICoroutineWork work);

        public abstract CoroutineStatus Status { get; }

        public abstract int CoroutineId { get; }

        public abstract string Name { get; }

        public abstract bool InCoroutine { get; }

        public abstract bool IsStart();

        public abstract bool IsStop();

        public abstract bool IsShutdown();

        public abstract Task Delay(int millisecondsDelay);

        public abstract Task Delay(TimeSpan delay);

        public abstract Task Repeat(int times, Action action);

        public abstract Task Repeat(int times, AsyncHandle handle);

        public abstract Task ExecUntil(AsyncHandle<bool> func);

        public abstract Task<T> ExecUntil<T>(AsyncHandle<CoroutineState<T>> func);

        public abstract Task RunUntil(Func<bool> func);

        public abstract Task<T> RunUntil<T>(Func<CoroutineState<T>> func);

        public abstract Task<bool> Shutdown(long millisecondsTimeout);

        public TaskScheduler TaskScheduler() => this;

        internal abstract void Tracked();

        internal abstract void Track();

        internal abstract bool InThread { get; }
    }

}
