// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Threading.Tasks;

namespace TnyFramework.Coroutines.Async
{

    internal interface ICoroutineTask
    {
        Task Invoke();
    }

    internal abstract class CoroutineTask<T> : ICoroutineTask
    {
        private Coroutine Coroutine { get; }

        protected TaskCompletionSource<T> Source { get; }

        protected CoroutineTask(bool createSource = true)
        {
            Source = createSource ? new TaskCompletionSource<T>() : null;
            Coroutine = null;
            Coroutine?.Track();
        }

        public abstract Task Invoke();
    }

    internal class CoroutineActionTask : CoroutineTask<int>
    {
        private readonly AsyncHandle handle;

        public Task SourceTask => Source?.Task;

        public CoroutineActionTask(AsyncHandle handle, bool createSource = true)
            : base(createSource)
        {
            this.handle = handle;
        }

        public override async Task Invoke()
        {
            try
            {
                await handle.Invoke();
                Source?.SetResult(1);
            } catch (Exception e)
            {
                Source?.SetException(e);
            }
        }
    }

    internal class CoroutineFuncTask<T> : CoroutineTask<T>
    {
        private readonly AsyncHandle<T> function;

        public CoroutineFuncTask(AsyncHandle<T> function, bool needSource = true)
            : base(needSource)
        {
            this.function = function;
        }

        public Task<T> SourceTask => Source?.Task;

        public override async Task Invoke()
        {
            try
            {
                var result = await function.Invoke();
                Source?.SetResult(result);
            } catch (Exception e)
            {
                Source?.SetException(e);
            }
        }
    }

    internal class ActionTask : CoroutineTask<int>
    {
        private readonly Action action;

        public ActionTask(Action action)
        {
            this.action = action;
        }

        public Task SourceTask => Source?.Task;

        public override Task Invoke()
        {
            try
            {
                action.Invoke();
                Source?.SetResult(1);
            } catch (Exception e)
            {
                Source?.SetException(e);
            }
            return Task.CompletedTask;
        }
    }

    internal class FuncTask<T> : CoroutineTask<T>
    {
        private readonly Func<T> func;

        public FuncTask(Func<T> func)
        {
            this.func = func;
        }

        public Task<T> SourceTask => Source?.Task;

        public override Task Invoke()
        {
            try
            {
                var result = func.Invoke();
                Source?.SetResult(result);
            } catch (Exception e)
            {
                Source?.SetException(e);
            }
            return Task.CompletedTask;
        }
    }

}
