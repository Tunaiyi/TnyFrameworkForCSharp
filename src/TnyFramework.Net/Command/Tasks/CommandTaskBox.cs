// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using TnyFramework.Coroutines.Async;
using TnyFramework.Net.Command.Processor;

namespace TnyFramework.Net.Command.Tasks
{

    public class CommandTaskBox : IAsyncExecutor
    {
        private const int STATUS_IDLE = 0;
        private const int STATUS_SUBMITTED = 1;

        private readonly ReaderWriterLockSlim boxLock = new ReaderWriterLockSlim();

        private readonly ConcurrentQueue<ICommandTask> taskQueue = new ConcurrentQueue<ICommandTask>();

        private readonly ICommandTaskBoxProcessor processor;

        private volatile object attachment;

        private volatile bool closed;

        public CommandTaskBox(ICommandTaskBoxProcessor processor)
        {
            this.processor = processor;
        }

        public bool IsEmpty => taskQueue.IsEmpty;

        public bool AddTask(ICommandTask task)
        {
            boxLock.EnterReadLock();
            try
            {
                if (closed)
                {
                    return false;
                }
                taskQueue.Enqueue(task);
                processor.Submit(this);
                return true;
            } finally
            {
                boxLock.ExitReadLock();
            }
        }

        public bool TakeOver(CommandTaskBox box)
        {
            if (closed)
            {
                return false;
            }
            boxLock.EnterReadLock();
            try
            {
                if (closed)
                {
                    return false;
                }
                if (!box.Close(out var remain))
                    return true;
                if (remain.Count < 0)
                    return true;
                foreach (var task in remain)
                {
                    taskQueue.Enqueue(task);
                }
                processor.Submit(this);
                return true;
            } finally
            {
                boxLock.ExitReadLock();
            }
        }

        private bool Close(out IList<ICommandTask> tasks)
        {
            if (closed)
            {
                tasks = null;
                return false;
            }
            boxLock.EnterWriteLock();
            try
            {
                if (closed)
                {
                    tasks = null;
                    return false;
                }
                closed = true;
                var returnQueue = ImmutableList.CreateRange(taskQueue);
                while (taskQueue.TryDequeue(out _))
                {
                }
                tasks = returnQueue;
                return true;
            } finally
            {
                boxLock.ExitWriteLock();
            }
        }

        public bool Poll(out ICommandTask task)
        {
            return taskQueue.TryDequeue(out task);
        }

        public TAttachment GetAttachment<TAttachment>()
        {
            if (attachment == null)
            {
                return default;
            }
            return (TAttachment) attachment;
        }

        public TAttachment SetAttachmentIfNull<TAttachment>(ICommandTaskBoxProcessor checkProcessor, TAttachment value)
        {
            if (processor != checkProcessor)
                return default;
            if (attachment != null)
                return default;
            lock (this)
            {
                if (attachment != null)
                    return default;
                attachment = value;
                return value;
            }
        }

        public TAttachment SetAttachmentIfNull<TAttachment>(ICommandTaskBoxProcessor checkProcessor, Func<TAttachment> func)
        {
            if (processor != checkProcessor)
                return default;
            if (attachment != null)
                return default;
            lock (this)
            {
                if (attachment != null)
                    return default;
                var value = func.Invoke();
                attachment = value;
                return value;
            }
        }

        public TAttachment SetAttachment<TAttachment>(ICommandTaskBoxProcessor checkProcessor, TAttachment value)
        {
            if (processor != checkProcessor)
                return default;
            lock (this)
            {
                attachment = value;
                return value;
            }
        }

        public TAttachment SetAttachment<TAttachment>(ICommandTaskBoxProcessor checkProcessor, Func<TAttachment> func)
        {
            if (processor != checkProcessor)
                return default;
            lock (this)
            {
                var value = func.Invoke();
                attachment = value;
                return value;
            }
        }

        public Task AsyncExec(AsyncHandle handle)
        {
            return processor.AsyncExec(this, handle);
        }

        public Task<T> AsyncExec<T>(AsyncHandle<T> function)
        {
            return processor.AsyncExec(this, function);
        }
    }

}
