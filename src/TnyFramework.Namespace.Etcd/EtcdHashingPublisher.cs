// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Threading.Tasks;
using TnyFramework.Codec;
using TnyFramework.Coroutines.Async;
using TnyFramework.Namespace.Sharding;

namespace TnyFramework.Namespace.Etcd
{

    public class EtcdHashingPublisher<TKey, TValue> : EtcdHashing<TValue>, IHashingPublisher<TKey, TValue>
    {
        private volatile TaskCompletionSource<ILessee> lesseeSource;

        private volatile ILessee lessee;

        private readonly IHasher<TValue> valueHasher;

        private readonly ICoroutine coroutine;

        public EtcdHashingPublisher(string path, long maxSlots, IHasher<TValue> valueHasher, ObjectMimeType<TValue> mineType,
            INamespaceExplorer explorer)
            : base(path, maxSlots, mineType, explorer)
        {
            this.valueHasher = valueHasher;
            coroutine = DefaultCoroutineFactory.Default.Create("EtcdHashingPublisher");
        }

        public Task<ILessee> Lease()
        {
            return Lease(60000);
        }

        public Task<ILessee> Lease(long ttl)
        {
            return coroutine.AsyncExec(async () => {
                var currentLessee = lessee;
                if (currentLessee != null)
                {
                    return currentLessee;
                }
                var currentSource = lesseeSource;
                if (currentSource != null)
                {
                    return await currentSource.Task;
                }
                currentSource = new TaskCompletionSource<ILessee>();
                try
                {
                    var result = await Explorer.Lease("Publisher#" + Path, ttl);
                    if (result != null)
                    {
                        lessee = result;
                    }
                    currentSource.SetResult(result);
                } catch (Exception e)
                {
                    currentSource.SetException(e);
                } finally
                {
                    lesseeSource = null;
                }
                return await currentSource.Task;
            });
        }

        public string PathOf(TKey key, TValue value)
        {
            var hashValue = ValueHash(value);
            return NamespacePathNames.NodePath(Path, SlotName(hashValue), key);
        }

        public Task<NameNode<TValue>> Publish(TKey key, TValue value)
        {
            var valuePath = PathOf(key, value);
            return lessee != null
                ? Explorer.Save(valuePath, MineType, value, lessee)
                : Explorer.Save(valuePath, MineType, value);
        }

        public Task<NameNode<TValue>> Operate(TKey key, TValue value, Publishing<TValue> publishing)
        {
            var valuePath = PathOf(key, value);
            return publishing(Explorer, valuePath, value, MineType, lessee);
        }

        public Task<NameNode<TValue>> PublishIfAbsent(TKey key, TValue value)
        {
            var valuePath = PathOf(key, value);
            return lessee != null
                ? Explorer.Add(valuePath, MineType, value, lessee)
                : Explorer.Add(valuePath, MineType, value);
        }

        public Task<NameNode<TValue>> PublishIfExist(TKey key, TValue value)
        {
            var valuePath = PathOf(key, value);
            return lessee != null
                ? Explorer.Update(valuePath, MineType, value, lessee)
                : Explorer.Update(valuePath, MineType, value);
        }

        public Task<NameNode<TValue>> Revoke(TKey key, TValue value)
        {
            var valuePath = PathOf(key, value);
            return Explorer.RemoveAndGet(valuePath, MineType);
        }

        private long ValueHash(TValue value)
        {
            return Math.Abs(valueHasher.Hash(value, 0, MaxSlots));
        }
    }

}
