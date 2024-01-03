// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Collections.Concurrent;
using System.Threading;

namespace TnyFramework.Common.Pools
{

    /// <summary>
    /// Default implementation of <see cref="ObjectPool{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type to pool objects for.</typeparam>
    /// <remarks>This implementation keeps a cache of retained objects. This means that if objects are returned when the pool has already reached "maximumRetained" objects they will be available to be Garbage Collected.</remarks>
    public class DefaultPool<T> : AnyPool<T> where T : class
    {
        private readonly Func<T> createFunc;
        private readonly Func<T, bool> returnFunc;
        private readonly int maxCapacity;
        private int numItems;

        private readonly ConcurrentQueue<T> items = new();
        private protected T? fastItem;

        /// <summary>
        /// Creates an instance of <see cref="DefaultPool{T}"/>.
        /// </summary>
        /// <param name="policy">The pooling policy to use.</param>
        public DefaultPool(IPooledPolicy<T> policy)
            : this(policy, Environment.ProcessorCount * 2)
        {
        }

        /// <summary>
        /// Creates an instance of <see cref="DefaultPool{T}"/>.
        /// </summary>
        /// <param name="policy">The pooling policy to use.</param>
        /// <param name="maximumRetained">The maximum number of objects to retain in the pool.</param>
        public DefaultPool(IPooledPolicy<T> policy, int maximumRetained)
        {
            // cache the target interface methods, to avoid interface lookup overhead
            createFunc = policy.Create;
            returnFunc = policy.Return;
            maxCapacity = maximumRetained - 1; // -1 to account for _fastItem
        }

        /// <inheritdoc />
        public override T Get()
        {
            var item = fastItem;
            if (item == null || Interlocked.CompareExchange(ref fastItem, null, item) != item)
            {
                if (items.TryDequeue(out item))
                {
                    Interlocked.Decrement(ref numItems);
                    return item;
                }

                // no object available, so go get a brand new one
                return createFunc();
            }

            return item;
        }

        /// <inheritdoc />
        public override void Return(T obj)
        {
            ReturnCore(obj);
        }

        /// <summary>
        /// Returns an object to the pool.
        /// </summary>
        /// <returns>true if the object was returned to the pool</returns>
        private protected bool ReturnCore(T obj)
        {
            if (!returnFunc(obj))
            {
                // policy says to drop this object
                return false;
            }

            if (fastItem != null || Interlocked.CompareExchange(ref fastItem, obj, null) != null)
            {
                if (Interlocked.Increment(ref numItems) <= maxCapacity)
                {
                    items.Enqueue(obj);
                    return true;
                }

                // no room, clean up the count and drop the object on the floor
                Interlocked.Decrement(ref numItems);
                return false;
            }

            return true;
        }
    }

}
