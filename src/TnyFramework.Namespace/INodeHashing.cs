// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System.Collections.Generic;
using System.Threading.Tasks;
using TnyFramework.Namespace.Sharding;

namespace TnyFramework.Namespace;

public interface INodeHashing<TNode> : ISharding<TNode>
    where TNode : IShardingNode
{
    /// <summary>
    /// 名字
    /// </summary>
    string Name { get; }

    /// <summary>
    /// 路径
    /// </summary>
    string Path { get; }

    Task<INodeHashing<TNode>> Start();

    Task<List<IPartition<TNode>>> Register(TNode node);

    Task<List<IPartition<TNode>>> Register(TNode node, ISet<long> slotIndexes);

    Task Shutdown();
}
