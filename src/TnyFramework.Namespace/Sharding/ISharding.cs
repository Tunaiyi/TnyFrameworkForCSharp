// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System.Collections.Generic;
using TnyFramework.Common.Event;
using TnyFramework.Namespace.Sharding.Listener;

namespace TnyFramework.Namespace.Sharding
{

    public interface ISharding<TNode> where TNode : IShardingNode
    {
        /// <summary>
        /// 最大槽数
        /// </summary>
        long MaxSlots { get; }

        /// <summary>
        /// 节点是否存在
        /// </summary>
        /// <param name="partition">分区</param>
        /// <returns>存在返回 Ture, 否则返回 False</returns>
        bool Contains(IPartition<TNode> partition);

        /// <summary>
        /// 获取节点对应的分区列表
        /// </summary>
        /// <param name="nodeId">Id 节点id</param>
        /// <returns>返回分区列表</returns>
        List<IPartition<TNode>> FindPartitions(string nodeId);

        /// <summary>
        /// 获取节点对应的分区列表
        /// </summary>
        /// <param name="nodeId">Id 节点id</param>
        /// <returns>返回分区列表</returns>
        List<ShardingRange<TNode>> FindRanges(string nodeId);

        /// <summary>
        /// 获取所有分区列表
        /// </summary>
        /// <returns>返回分区列表</returns>
        List<ShardingRange<TNode>> GetAllRanges();

        /// <summary>
        /// 获取指定槽位的前一个分区(不包含 slot 分区)
        /// </summary>
        /// <param name="slot">指定槽位</param>
        /// <returns>返回前一个分区</returns>
        IPartition<TNode>? PrevPartition(long slot);

        /// <summary>
        /// 获取指定槽位的后一个分区(不包含 slot 分区)
        /// </summary>
        /// <param name="slot">指定槽位</param>
        /// <returns>返回后一个分区</returns>
        IPartition<TNode>? NextPartition(long slot);

        /// <summary>
        /// @return 获取所有分区
        /// </summary>
        List<IPartition<TNode>> GetAllPartitions();

        /// <summary>
        /// 更具 Key 定位节点
        /// </summary>
        /// <param name="key">定位的 key</param>
        /// <returns>返回节点</returns>
        IPartition<TNode>? Locate(string key);

        /// <summary>
        /// 更具 Key 定位返回指定数量的节点
        /// </summary>
        /// <param name="key">  定位的 key</param>
        /// <param name="count">返回节点数量</param>
        /// <returns>返回节点列表</returns>
        List<IPartition<TNode>> Locate(string key, int count);

        /// <summary>
        /// @return 返回丰满区数量
        /// </summary>
        int PartitionSize();

        ///  <summary>
        /// 分区改变事件
        /// </summary>
        /// <returns>分区改变事件</returns>
        IEventWatch<ShardingOnChange<TNode>> ChangeEvent { get; }

        ///  <summary>
        /// 分区删除事件
        /// </summary>
        /// <returns>分区改变事件</returns>
        IEventWatch<ShardingOnRemove<TNode>> RemoveEvent { get; }
    }

}
