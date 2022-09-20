// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System.Collections.Generic;

namespace TnyFramework.Namespace.Sharding.Listener
{

    /// <summary>
    /// 增加分片改变
    /// </summary>
    /// <typeparam name="TNode">类型</typeparam>
    public delegate void ShardingOnChange<TNode>(ISharding<TNode> sharding, IReadOnlyCollection<IPartition<TNode>> partitions)
        where TNode : IShardingNode;

    /// <summary>
    /// 移除分片改变
    /// </summary>
    /// <typeparam name="TNode">类型</typeparam>
    public delegate void ShardingOnRemove<TNode>(ISharding<TNode> sharding, IReadOnlyCollection<IPartition<TNode>> partitions)
        where TNode : IShardingNode;

}
