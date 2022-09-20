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
