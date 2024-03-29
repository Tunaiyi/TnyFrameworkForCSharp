// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System.Threading.Tasks;
using TnyFramework.Codec;

namespace TnyFramework.Namespace;

public delegate Task<NameNode<TValue>> Publishing<TValue>(
    INamespaceExplorer explorer, string path, TValue value, ObjectMimeType<TValue> mineType, ILessee? lessee);

public interface IHashingPublisher<in TKey, TValue>
{
    string Path { get; }

    ObjectMimeType<TValue> MineType { get; }

    Task<ILessee> Lease();

    Task<ILessee> Lease(long ttl);

    string PathOf(TKey key, TValue value);

    Task<NameNode<TValue>> Publish(TKey key, TValue value);

    Task<NameNode<TValue>> Operate(TKey key, TValue value, Publishing<TValue> publishing);

    Task<NameNode<TValue>> PublishIfAbsent(TKey key, TValue value);

    Task<NameNode<TValue>> PublishIfExist(TKey key, TValue value);

    Task<NameNode<TValue>> Revoke(TKey key, TValue value);
}
