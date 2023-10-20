using System;
using System.Threading;
using System.Threading.Tasks;

namespace TnyFramework.Common.Extensions
{

    public static class TaskSchedulerExtensions
    {
        private static readonly TaskCreationOptions OPTIONS = TaskCreationOptions.None;
        private static readonly CancellationToken TOKEN = CancellationToken.None;

        public static Task StartNew(this TaskScheduler scheduler, Action action)
        {
            return Task.Factory.StartNew(action, TOKEN, OPTIONS, scheduler);
        }

        public static Task StartNew(this TaskScheduler scheduler, Action<object?> action, object? state)
        {
            return Task.Factory.StartNew(action, state, TOKEN, OPTIONS, scheduler);
        }

        public static Task<TResult> StartNew<TResult>(this TaskScheduler scheduler, Func<TResult> function)
        {
            return Task.Factory.StartNew(function, TOKEN, OPTIONS, scheduler);
        }

        public static Task<TResult> StartNew<TResult>(this TaskScheduler scheduler, Func<object?, TResult> function, object? state)
        {
            return Task.Factory.StartNew(function, state, TOKEN, OPTIONS, scheduler);
        }
    }

}
