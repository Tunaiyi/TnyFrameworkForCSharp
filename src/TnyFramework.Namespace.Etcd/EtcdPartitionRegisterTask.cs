using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TnyFramework.Common.Logger;
using TnyFramework.Coroutines.Async;
using TnyFramework.Namespace.Sharding;

namespace TnyFramework.Namespace.Etcd
{

    internal class EtcdPartitionRegisterTask
    {
        
        internal const int REGISTER_RESULT_SUCCESS = 1;
        internal const int REGISTER_RESULT_FAILED = 2;
        internal  const int REGISTER_RESULT_RETRY = 3;
        internal  const int REGISTER_RESULT_CANCEL = 4;
    }
    internal class EtcdPartitionRegisterTask<TNode> : EtcdPartitionRegisterTask
        where TNode : IShardingNode
    {

        private static readonly ILogger LOGGER = LogFactory.Logger<EtcdPartitionRegisterTask<TNode>>();

        internal PartitionSlot<TNode> Partition { get; }

        private volatile bool sync;

        private volatile bool closed;

        private readonly ICoroutine coroutine;

        private readonly bool rehash;

        private readonly EtcdNodeHashing<TNode> hashing;

        private volatile TaskCompletionSource<ShardingPartition<TNode>> source;

        internal EtcdPartitionRegisterTask(EtcdNodeHashing<TNode> hashing, PartitionSlot<TNode> partition, bool rehash)
        {
            Partition = partition;
            this.rehash = rehash;
            this.hashing = hashing;
            coroutine = DefaultCoroutineFactory.Default.Create("PartitionTask");
        }

        internal async Task Close()
        {
            await coroutine.AsyncExec(() => {
                closed = true;
                source?.SetCanceled();
                return Task.CompletedTask;
            });
        }

        internal Task<ShardingPartition<TNode>> Register()
        {
            return coroutine.AsyncExec(async () => {
                var current = source;
                if (closed)
                {
                    if (current != null)
                    {
                        return await source.Task;
                    }
                    current = new TaskCompletionSource<ShardingPartition<TNode>>();
                    current.SetCanceled();
                    return await current.Task;
                }
                if (sync && current != null)
                {
                    return await current.Task;
                }
                sync = true;
                if (source == null)
                {
                    source = new TaskCompletionSource<ShardingPartition<TNode>>();
                }
                await DoRegister(source, false);
                return await source.Task;
            });
        }

        private void TryAgain(TaskCompletionSource<ShardingPartition<TNode>> currentSource)
        {
            coroutine.AsyncExec(async () => {
                await Task.Delay(5000);
                if (closed && sync || currentSource.Task.IsCompleted)
                {
                    return;
                }
                sync = true;
                await DoRegister(currentSource, true);
            });
        }

        private async Task DoRegister(TaskCompletionSource<ShardingPartition<TNode>> currentSource, bool hash)
        {
            try
            {
                if (hash && rehash)
                {
                    Partition.Hash(hashing.NodeHasher, hashing.MaxSlots);
                }
                var result = await hashing.RegisterPartition(this);
                OnCompleted(currentSource, result);
            } catch (Exception e)
            {
                LOGGER.LogError(e, "DoRegister {} exception", Partition);
                sync = false;
                OnCompleted(currentSource, REGISTER_RESULT_RETRY);
            }
        }

        private void OnCompleted(TaskCompletionSource<ShardingPartition<TNode>> currentSource, int result)
        {
            try
            {
                switch (result)
                {
                    case REGISTER_RESULT_SUCCESS:
                        currentSource.SetResult(Partition);
                        return;
                    case REGISTER_RESULT_CANCEL:
                        currentSource.SetCanceled();
                        return;
                    case REGISTER_RESULT_RETRY:
                        TryAgain(currentSource);
                        return;
                    case REGISTER_RESULT_FAILED:
                        if (rehash)
                        {
                            TryAgain(currentSource);
                        } else
                        {
                            currentSource.SetResult(null);
                        }
                        return;
                }
            } finally
            {
                sync = true;
                if (currentSource.Task.IsCompleted && currentSource == source)
                {
                    source = null;
                }
            }
        }
    }

}
