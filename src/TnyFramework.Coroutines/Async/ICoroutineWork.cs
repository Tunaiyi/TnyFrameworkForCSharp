// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TnyFramework.Common.Logger;

namespace TnyFramework.Coroutines.Async
{

    public interface ICoroutineWork
    {
        Task Invoke();

        Task AwaitTask { get; }
    }

    internal abstract class AbstractCoroutineWork<T> : ICoroutineWork
    {
        private readonly ILogger logger = LogFactory.Logger<ICoroutineWork>();

        private Coroutine? Coroutine { get; }

        protected TaskCompletionSource<T>? Source { get; }

        protected AbstractCoroutineWork(bool createSource = true)
        {
            Source = createSource ? new TaskCompletionSource<T>() : null;
            Coroutine = null;
            Coroutine?.Track();
        }

        protected void HandleException(Exception cause)
        {
            var current = Coroutine.CurrentCoroutine;
            if (current == null)
            {
                logger.LogWarning(cause, "HandleException current coroutine is null");
                return;
            }
            var name = current.Name;
            var id = current.CoroutineId;
            logger.LogWarning(cause, "Coroutine {Name} [{Id}] Invoke exception", name, id);
        }

        public abstract Task Invoke();

        public abstract Task AwaitTask { get; }
    }

    internal class CoroutineActionWork : AbstractCoroutineWork<int>
    {
        private readonly AsyncHandle handle;

        public override Task AwaitTask => Source?.Task!;

        public CoroutineActionWork(AsyncHandle handle, bool createSource = true)
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
                HandleException(e);
            }
        }
    }

    internal class CoroutineFuncWork<T> : AbstractCoroutineWork<T>
    {
        private readonly AsyncHandle<T> function;

        public CoroutineFuncWork(AsyncHandle<T> function, bool needSource = true)
            : base(needSource)
        {
            this.function = function;
        }

        public override Task AwaitTask => Source?.Task!;

        public Task<T> AwaitTypeTask => Source?.Task!;

        public override async Task Invoke()
        {
            try
            {
                var result = await function.Invoke();
                Source?.SetResult(result);
            } catch (Exception e)
            {
                Source?.SetException(e);
                HandleException(e);
            }
        }
    }

    internal class ActionWork : AbstractCoroutineWork<int>
    {
        private readonly Action action;

        public ActionWork(Action action)
        {
            this.action = action;
        }

        public override Task AwaitTask => Source?.Task!;

        public override Task Invoke()
        {
            try
            {
                action.Invoke();
                Source?.SetResult(1);
            } catch (Exception e)
            {
                Source?.SetException(e);
                HandleException(e);
            }
            return Task.CompletedTask;
        }
    }

    internal class FuncWork<T> : AbstractCoroutineWork<T>
    {
        private readonly Func<T> func;

        public FuncWork(Func<T> func)
        {
            this.func = func;
        }

        public override Task AwaitTask => Source?.Task!;

        public override Task Invoke()
        {
            try
            {
                var result = func.Invoke();
                Source?.SetResult(result);
            } catch (Exception e)
            {
                Source?.SetException(e);
                HandleException(e);
            }
            return Task.CompletedTask;
        }
    }

}
