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
using TnyFramework.Net.Message;

namespace TnyFramework.Net.Command.Tasks
{

    public class CommandBox
    {
        private readonly ReaderWriterLockSlim boxLock = new();

        private readonly ConcurrentQueue<ICommand> commandQueue = new();

        private readonly ICommandExecutor executor;

        private volatile bool closed;

        public CommandBox(ICommandExecutorFactory executorFactory)
        {
            executor = executorFactory.CreateCommandExecutor(this);
        }

        public bool IsEmpty => commandQueue.IsEmpty;

        public TaskScheduler TaskScheduler => executor.TaskScheduler;

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
                executor.TrySummit();
                return true;
            } finally
            {
                boxLock.ExitReadLock();
            }
        }

        public bool AddCommand(IRpcMessageEnterContext rpcMessageContext)
        {
            return DoAddCommand(() => CreateCommand(rpcMessageContext));
        }

        private ICommand? CreateCommand(IRpcMessageEnterContext rpcMessageContext)
        {
            var message = rpcMessageContext.NetMessage;
            switch (message.Mode)
            {
                case MessageMode.Push:
                case MessageMode.Request:
                case MessageMode.Response:
                    var context = rpcMessageContext.NetworkContext;
                    var dispatcher = context.MessageDispatcher;
                    return dispatcher.Dispatch(rpcMessageContext);
                case MessageMode.Ping:
                    var tunnel = rpcMessageContext.NetTunnel;
                    rpcMessageContext.Complete();
                    return new RunnableCommand(tunnel.Pong);
                case MessageMode.Pong:
                default:
                    rpcMessageContext.CompleteSilently();
                    break;
            }
            return null;
        }

        private bool DoAddCommand(Func<ICommand?> commandFunc)
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
                executor.TrySummit();
                return true;
            } finally
            {
                boxLock.ExitReadLock();
            }
        }

        public Task AsyncExec(AsyncHandle handle)
        {
            return executor.AsyncExec(handle);
        }

        public Task<T> AsyncExec<T>(AsyncHandle<T> function)
        {
            return executor.AsyncExec(function);
        }

        public bool TakeOver(CommandBox box)
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
                executor.TrySummit();
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
                tasks = null!;
                return false;
            }
            boxLock.EnterWriteLock();
            try
            {
                if (closed)
                {
                    tasks = null!;
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
            return commandQueue.TryDequeue(out task!);
        }
    }

}
