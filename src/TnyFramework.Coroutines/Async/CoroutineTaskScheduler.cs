// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

#nullable enable
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.ObjectPool;

namespace TnyFramework.Coroutines.Async
{

    public abstract class CoroutineTaskScheduler : TaskScheduler
    {
        private Coroutine coroutine = null!;

        protected CoroutineTaskScheduler()
        {
        }

        protected CoroutineTaskScheduler(Coroutine coroutine)
        {
            this.coroutine = coroutine;
        }

        protected Coroutine Coroutine {
            set => coroutine = value;
        }

        protected override IEnumerable<Task> GetScheduledTasks()
        {
            return Array.Empty<Task>();
        }

        protected override void QueueTask(Task task)
        {
            coroutine.AsyncExec(TaskQueueNode.Factory(this, task));
        }

        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            if (taskWasPreviouslyQueued || !coroutine.InCoroutine)
            {
                return false;
            }
            return TryExecuteTask(task);
        }

        private sealed class TaskQueueNode : ICoroutineWork
        {
            private class TaskQueueNodePoolPolicy : PooledObjectPolicy<TaskQueueNode>
            {
                public override TaskQueueNode Create() => new();

                public override bool Return(TaskQueueNode obj) => true;
            }

            private static readonly DefaultObjectPool<TaskQueueNode> POOL = new(new TaskQueueNodePoolPolicy());

            static TaskQueueNode()
            {
                for (var i = 0; i < 10; i++)
                {
                    POOL.Return(new TaskQueueNode());
                }
            }

            public static TaskQueueNode Factory(CoroutineTaskScheduler scheduler, Task task)
            {
                var node = POOL.Get();
                node.Scheduler = scheduler;
                node.Task = task;
                return node;
            }

            private CoroutineTaskScheduler? Scheduler { get; set; }

            private Task? Task { get; set; }

            public Task AwaitTask => Task!;

            public Task Invoke()
            {
                try
                {
                    Scheduler!.TryExecuteTask(Task!);
                } finally
                {
                    Scheduler = null;
                    Task = null;
                    POOL.Return(this);
                }
                return Task.CompletedTask;
            }
        }
    }

}
