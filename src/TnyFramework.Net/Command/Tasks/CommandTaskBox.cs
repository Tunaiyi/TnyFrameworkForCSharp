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
using TnyFramework.Net.Command.Dispatcher;
using TnyFramework.Net.Command.Processor;
using TnyFramework.Net.Message;

namespace TnyFramework.Net.Command.Tasks
{

    public class CommandTaskBox : IAsyncExecutor
    {
        private const int STATUS_IDLE = 0;
        private const int STATUS_SUBMITTED = 1;

        private readonly ReaderWriterLockSlim boxLock = new ReaderWriterLockSlim();

        private readonly ConcurrentQueue<ICommand> commandQueue = new ConcurrentQueue<ICommand>();

        private readonly ICommandTaskBoxProcessor processor;

        private volatile object attachment;

        private volatile bool closed;

        public CommandTaskBox(ICommandTaskBoxProcessor processor)
        {
            this.processor = processor;
        }

        public bool IsEmpty => commandQueue.IsEmpty;

        public bool AddCommand(IRpcProviderContext rpcContext)
        {
            return DoAddCommand(() => CreateCommand(rpcContext));
        }

        public bool AddCommand(ICommand command)
        {
            boxLock.EnterReadLock();
            try
            {
                if (closed)
                {
                    return false;
                }
                commandQueue.Enqueue(command);
                processor.Submit(this);
                return true;
            } finally
            {
                boxLock.ExitReadLock();
            }
        }

        private ICommand CreateCommand(IRpcProviderContext rpcContext)
        {
            var message = rpcContext.NetMessage;
            switch (message.Mode)
            {
                case MessageMode.Push:
                case MessageMode.Request:
                case MessageMode.Response:
                    var context = rpcContext.NetworkContext;
                    var dispatcher = context.MessageDispatcher;
                    return dispatcher.Dispatch(rpcContext);
                case MessageMode.Ping:
                    var tunnel = rpcContext.NetTunnel;
                    rpcContext.Complete();
                    return new RunnableCommand(tunnel.Pong);
                case MessageMode.Pong:
                default:
                    rpcContext.CompleteSilently();
                    break;
            }
            return null;
        }

        public Task AsyncExec(AsyncHandle handle)
        {
            return processor.AsyncExec(this, handle);
        }

        public Task<T> AsyncExec<T>(AsyncHandle<T> function)
        {
            return processor.AsyncExec(this, function);
        }

        private bool DoAddCommand(Func<ICommand> commandFunc)
        {
            boxLock.EnterReadLock();
            try
            {
                if (closed)
                {
                    return false;
                }
                var command = commandFunc.Invoke();
                if (command == null)
                    return false;
                commandQueue.Enqueue(command);
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
                    commandQueue.Enqueue(task);
                }
                processor.Submit(this);
                return true;
            } finally
            {
                boxLock.ExitReadLock();
            }
        }

        private bool Close(out IList<ICommand> tasks)
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
                var returnQueue = ImmutableList.CreateRange(commandQueue);
                while (commandQueue.TryDequeue(out _))
                {
                }
                tasks = returnQueue;
                return true;
            } finally
            {
                boxLock.ExitWriteLock();
            }
        }

        public bool Poll(out ICommand task)
        {
            return commandQueue.TryDequeue(out task);
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

        public TAttachment SetAttachmentIfNull<TAttachment>(ICommandTaskBoxProcessor checkProcessor,
            Func<TAttachment> func)
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
