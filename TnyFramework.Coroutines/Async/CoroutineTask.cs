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
        private readonly CoroutineAction action;

        public Task SourceTask => Source?.Task;


        public CoroutineActionTask(CoroutineAction action, bool createSource = true)
            : base(createSource)
        {
            this.action = action;
        }


        public override async Task Invoke()
        {
            try
            {
                await action.Invoke();
                Source?.SetResult(1);
            } catch (System.Exception e)
            {
                Source?.SetException(e);
            }
        }
    }

    internal class CoroutineFuncTask<T> : CoroutineTask<T>
    {
        private readonly CoroutineFunc<T> function;


        public CoroutineFuncTask(CoroutineFunc<T> function, bool needSource = true)
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
            } catch (System.Exception e)
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
            } catch (System.Exception e)
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
            } catch (System.Exception e)
            {
                Source?.SetException(e);
            }
            return Task.CompletedTask;
        }
    }
}
