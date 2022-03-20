using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using TnyFramework.Net.Command.Processor;
namespace TnyFramework.Net.Command.Tasks
{
    public class CommandTaskBox
    {
        private const int STATUS_IDLE = 0;
        private const int STATUS_SUBMITTED = 1;

        private readonly ReaderWriterLockSlim boxLock = new ReaderWriterLockSlim();

        private ICommandTaskBoxProcessor processor;

        private volatile bool closed = false;

        private ConcurrentQueue<ICommandTask> taskQueue = new ConcurrentQueue<ICommandTask>();

        private volatile object attachment;


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
            return (TAttachment)attachment;
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
    }
}
