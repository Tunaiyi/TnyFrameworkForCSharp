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

    public static class TaskSchedulerExtensions
    {
        private const TaskCreationOptions OPTIONS = TaskCreationOptions.None;
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

        public static Task StartNew<TState>(this TaskScheduler scheduler, Action<TState> action, TState state)
        {
            return Task.Factory.StartNew(s => action((TState) s!), state, TOKEN, OPTIONS, scheduler);
        }

        public static Task<TResult> StartNew<TResult>(this TaskScheduler scheduler, Func<object?, TResult> function, object? state)
        {
            return Task.Factory.StartNew(function, state, TOKEN, OPTIONS, scheduler);
        }

        public static Task<TResult> StartNew<TResult, TState>(this TaskScheduler scheduler, Func<TState, TResult> function, TState state)
        {
            return Task.Factory.StartNew(s => function((TState) s!), state, TOKEN, OPTIONS, scheduler);
        }

        public static Task StartAwaitNew(this TaskScheduler scheduler, Func<Task> action)
        {
            return Task.Factory.StartNew(action, TOKEN, OPTIONS, scheduler).Unwrap();
        }

        public static Task StartAwaitNew(this TaskScheduler scheduler, Func<object?, Task> action, object? state)
        {
            return Task.Factory.StartNew(action, state, TOKEN, OPTIONS, scheduler).Unwrap();
        }

        public static Task<TResult> StartAwaitNew<TResult>(this TaskScheduler scheduler, Func<Task<TResult>> function)
        {
            return Task.Factory.StartNew(function, TOKEN, OPTIONS, scheduler).Unwrap();
        }

        public static Task StartAwaitNew<TState>(this TaskScheduler scheduler, Func<TState, Task> action, TState state)
        {
            return Task.Factory.StartNew(s => action((TState) s!), state, TOKEN, OPTIONS, scheduler).Unwrap();
        }

        public static Task<TResult> StartAwaitNew<TResult>(this TaskScheduler scheduler, Func<object?, Task<TResult>> function, object? state)
        {
            return Task.Factory.StartNew(function, state, TOKEN, OPTIONS, scheduler).Unwrap();
        }

        public static Task<TResult> StartAwaitNew<TResult, TState>(this TaskScheduler scheduler, Func<TState, Task<TResult>> function, TState state)
        {
            return Task.Factory.StartNew(s => function((TState) s!), state, TOKEN, OPTIONS, scheduler).Unwrap();
        }
    }

}
