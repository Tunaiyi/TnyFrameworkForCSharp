// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace TnyFramework.Common.Pools
{

    /// <summary>
    /// Default implementation for <see cref="PooledPolicy{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of object which is being pooled.</typeparam>
    public class DefaultPooledPolicy<T> : PooledPolicy<T> where T : class, new()
    {
        /// <inheritdoc />
        public override T Create()
        {
            return new T();
        }

        /// <inheritdoc />
        public override bool Return(T obj)
        {
            if (obj is IResettable resettable)
            {
                return resettable.TryReset();
            }

            return true;
        }
    }

}
