// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace TnyFramework.Namespace.Sharding
{

    public interface IPartition
    {
        /// <summary>
        /// Partition key 值
        /// </summary>
        string Key { get; }

        /// <summary>
        /// Node key 值
        /// </summary>
        string NodeKey { get; }

        /// <summary>
        /// 节点序号
        /// </summary>
        int Index { get; }

        /// <summary>
        /// 节点槽位
        /// </summary>
        long Slot { get; }

        /// <summary>
        /// 获取节点
        /// </summary>
        /// <returns>返回节点</returns>
        IShardingNode GetNode();

        /// <summary>
        /// 获取节点
        /// </summary>
        /// <typeparam name="TValue">节点类型</typeparam>
        /// <returns>返回节点</returns>
        TValue GetNode<TValue>() where TValue : IShardingNode;
    }

    public interface IPartition<out TNode> : IPartition
        where TNode : IShardingNode
    {
        /// <summary>
        /// Node
        /// </summary>
        TNode Node { get; }
    }

}
