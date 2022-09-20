using System;
using System.Threading;
using System.Threading.Tasks;

namespace TnyFramework.Common.Extensions
{

    public static class TaskExtensions
    {
        public static async Task<bool> ForTimeout(this Task task, int timeout, Action onTimeout = null)
        {
            return await task.ForTimeout(Task.Delay(timeout), onTimeout);
        }

        public static async Task<bool> ForTimeout(this Task task, int timeout, CancellationToken cancellationToken, Action onTimeout = null)
        {
            return await task.ForTimeout(Task.Delay(timeout, cancellationToken), onTimeout);
        }

        public static async Task<bool> ForTimeout(this Task task, TimeSpan timeout, Action onTimeout = null)
        {
            return await task.ForTimeout(Task.Delay(timeout), onTimeout);
        }

        public static async Task<bool> ForTimeout(this Task task, TimeSpan timeout, CancellationToken cancellationToken, Action onTimeout = null)
        {

            return await task.ForTimeout(Task.Delay(timeout, cancellationToken), onTimeout);
        }

        public static async Task<bool> ForTimeout(this Task task, Task timeoutTask, Action onTimeout = null)
        {
            if (await Task.WhenAny(task, timeoutTask) != task)
                return false;
            onTimeout?.Invoke();
            return true;
        }

        public static async Task<TimeoutWait<TResult>> ForTimeout<TResult>(this Task<TResult> task, int timeout, Action onTimeout = null)
        {
            return await task.ForTimeout(Task.Delay(timeout), onTimeout);
        }

        public static async Task<TimeoutWait<TResult>> ForTimeout<TResult>(this Task<TResult> task, int timeout, CancellationToken cancellationToken,
            Action onTimeout = null)
        {
            return await task.ForTimeout(Task.Delay(timeout, cancellationToken), onTimeout);
        }

        public static async Task<TimeoutWait<TResult>> ForTimeout<TResult>(this Task<TResult> task, TimeSpan timeout, Action onTimeout = null)
        {
            return await task.ForTimeout(Task.Delay(timeout), onTimeout);
        }

        public static async Task<TimeoutWait<TResult>> ForTimeout<TResult>(this Task<TResult> task, TimeSpan timeout,
            CancellationToken cancellationToken, Action onTimeout = null)
        {
            return await task.ForTimeout(Task.Delay(timeout, cancellationToken), onTimeout);
        }

        public static async Task<TimeoutWait<TResult>> ForTimeout<TResult>(this Task<TResult> task, Task timeoutTask, Action onTimeout = null)
        {
            if (await Task.WhenAny(task, timeoutTask) != task)
            {
                return new TimeoutWait<TResult>(false, default);
            }
            onTimeout?.Invoke();
            return new TimeoutWait<TResult>(true, task.Result);
        }
    }

    public readonly struct TimeoutWait<TResult>
    {
        public bool Success { get; }

        public bool Failure => !Success;

        public TResult Result { get; }

        internal TimeoutWait(bool success, TResult result)
        {
            Success = success;
            Result = result;
        }
    }

}
