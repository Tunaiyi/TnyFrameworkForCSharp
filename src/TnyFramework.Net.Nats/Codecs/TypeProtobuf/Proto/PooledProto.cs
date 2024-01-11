// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using Microsoft.Extensions.ObjectPool;

namespace TnyFramework.Net.Nats.Codecs.TypeProtobuf.Proto
{

    public class PooledProtoPolicy<TProto> : IPooledObjectPolicy<TProto> where TProto : PooledProto<TProto>, new()
    {
        public TProto Create()
        {
            return new TProto();
        }

        public bool Return(TProto obj)
        {
            if (obj is IPooledProto proto)
            {
                proto.Clear();
            }
            return true;
        }
    }

    internal interface IPooledProto : IDisposable
    {
        internal void Clear();
    }

    public abstract class PooledProto<TProto> : IPooledProto where TProto : PooledProto<TProto>, new()
    {
        private static readonly ObjectPool<TProto> POOLS =
            new DefaultObjectPool<TProto>(new PooledProtoPolicy<TProto>());

        public static TProto Get()
        {
            return POOLS.Get();
        }

        void IPooledProto.Clear()
        {
            DoClear();
        }

        protected abstract void DoClear();

        public void Dispose()
        {
            if (this is TProto proto)
            {
                POOLS.Return(proto);
            }
        }
    }

}
