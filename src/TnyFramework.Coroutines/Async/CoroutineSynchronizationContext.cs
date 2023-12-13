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
using Microsoft.Extensions.Logging;
using TnyFramework.Common.Logger;

namespace TnyFramework.Coroutines.Async
{

    /// <summary>
    /// 协程SynchronizationContext
    /// </summary>
    public class CoroutineSynchronizationContext : SynchronizationContext
    {
        /// <summary>
        /// 协程对象
        /// </summary>
        private readonly Coroutine coroutine;

        /// <summary>
        /// 初始化 Context
        /// </summary>
        public static void InitializeSynchronizationContext()
        {
            var current = Current;
            if (current is CoroutineSynchronizationContext)
                return;
            var coroutine = new DefaultCoroutine();
            var synchronizationContext = new CoroutineSynchronizationContext(coroutine);
            SetSynchronizationContext(synchronizationContext);
        }

        /// <summary>
        /// 初始化 Context
        /// </summary>
        public static void InitializeSynchronizationContext(Coroutine coroutine)
        {
            var current = Current;
            if (current is CoroutineSynchronizationContext)
                return;
            var synchronizationContext = new CoroutineSynchronizationContext(coroutine);
            SetSynchronizationContext(synchronizationContext);
        }

        // internal static void InitializeSynchronizationContext()
        // {
        // }
        //
        // internal static void InitializeSynchronizationContext(ICoroutine coroutine)
        // {
        //     var current = SynchronizationContext.Current;
        //     if (current is CoroutineSynchronizationContext)
        //         return;
        //     if (!(coroutine is Coroutine value))
        //         return;
        //     SynchronizationContext.SetSynchronizationContext(value.AsSynchronizationContext());
        //     CURRENT_COROUTINE.Value = value;
        // }

        internal CoroutineSynchronizationContext(Coroutine coroutine)
        {
            this.coroutine = coroutine;
        }

        // private int Id => coroutine.Id;
        //
        // private string Name => coroutine.Name;

        public override SynchronizationContext CreateCopy()
        {
            return new CoroutineSynchronizationContext(coroutine);
        }

        public override void OperationStarted()
        {
            coroutine.Track();
        }

        public override void OperationCompleted()
        {
            coroutine.Tracked();
        }

        public override void Post(SendOrPostCallback callback, object? state)
        {
            coroutine.AsyncExec(new CoroutineSynchronizationContextWork(callback, this, state));
        }

        public override void Send(SendOrPostCallback callback, object? state)
        {
            if (coroutine.InThread)
            {
                callback(state);
            } else
            {
                var work = new CoroutineSynchronizationContextWork(callback, this, state);
                coroutine.AsyncExec(work);
                work.AwaitTask.Wait();
            }
        }

        public override string ToString()
        {
            return $"{coroutine}";
        }
    }

    public class CoroutineSynchronizationContextWork : ICoroutineWork
    {
        private static readonly ILogger LOGGER = LogFactory.Logger<CoroutineSynchronizationContextWork>();

        private readonly SendOrPostCallback callback;

        private readonly object? state;

#if NETSTANDARD2_1
        private readonly TaskCompletionSource<object> completion;
#else
        private readonly TaskCompletionSource completion;
#endif

        private readonly SynchronizationContext context;

        public CoroutineSynchronizationContextWork(SendOrPostCallback callback, SynchronizationContext context, object? state)
        {
            this.context = context;
            this.callback = callback;
            this.state = state;
#if NETSTANDARD2_1
            completion = new TaskCompletionSource<object>();
#else
            completion = new TaskCompletionSource();
#endif
        }

        public Task AwaitTask => completion.Task;

        Task ICoroutineWork.Invoke()
        {
            var currentContext = SynchronizationContext.Current;
            try
            {
                SynchronizationContext.SetSynchronizationContext(context);
                callback(state);
#if NETSTANDARD2_1
                completion.SetResult(null!);
#else
                completion.SetResult();
#endif
            } catch (Exception e)
            {
                completion.SetException(e);
                var thread = Thread.CurrentThread;
                LOGGER.LogError(e, "ThreadName {TName} - {TId} Run exception", thread.Name, thread.ManagedThreadId);
            } finally
            {
                SynchronizationContext.SetSynchronizationContext(currentContext);
            }
            return Task.CompletedTask;
        }
    }

}
