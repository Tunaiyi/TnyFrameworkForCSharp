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
using Microsoft.Extensions.ObjectPool;
using TnyFramework.Net.Message;

namespace TnyFramework.Net.Nats.Transports
{

    public abstract class NatsCommand : INatsCommand, INatsCommandCompletable
    {
        private string topic = null!;
        private string accessKey = null!;

        private volatile TaskCompletionSource? source;

        public string Topic => topic;

        public string AccessKey => accessKey;

        public abstract IMessage? Message { get; }

        public abstract NatsAction Action { get; }

        public abstract void Dispose();

        protected NatsCommand()
        {
        }

        protected void Init(NatsTransport transport)
        {
            topic = transport.Topic;
            accessKey = transport.LocalAccessKey;
        }

        protected void Init(string topic, string accessKey)
        {
            this.topic = topic;
            this.accessKey = accessKey;
        }

        void INatsCommand.Clear()
        {
            topic = null!;
            accessKey = null!;
            source = null!;
            DoClear();
        }

        protected abstract void DoClear();

        Task INatsCommandCompletable.Task => source?.Task ?? Task.CompletedTask;

        public bool WaitWritten => source != null;

        void INatsCommandCompletable.Complete()
        {
            source?.TrySetResult();
        }

        public void Cancel()
        {
            source?.TrySetCanceled();
        }

        void INatsCommandCompletable.SetException(Exception exception)
        {
            source?.TrySetException(exception);
        }

        Task INatsCommandCompletable.BeWait()
        {
            var current = source;
            if (current != null)
            {
                return current.Task;
            }
            Interlocked.CompareExchange(ref source, new TaskCompletionSource(), null);
            return source.Task;
        }
    }

    public abstract class NatsCommand<T> : NatsCommand where T : NatsCommand<T>, new()
    {
        private static readonly IPooledObjectPolicy<T> POLICY = new NatsCommandObjectPolicy<T>();
        private static readonly ObjectPool<T> COMMAND_POOL = new DefaultObjectPool<T>(POLICY);

        private volatile int rent;

        protected static T Get()
        {
            while (true)
            {
                var command = COMMAND_POOL.Get();
                if (command.Rant())
                    return command;
            }
        }

        private bool Rant()
        {
            return Interlocked.CompareExchange(ref rent, 0, 1) == 0;
        }

        public override void Dispose()
        {
            if (Interlocked.CompareExchange(ref rent, 1, 0) == 1)
            {
                COMMAND_POOL.Return((T) this);
            }
        }
    }

}
