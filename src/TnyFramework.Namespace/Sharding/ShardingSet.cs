// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System.Collections.Generic;

namespace TnyFramework.Namespace.Sharding
{

    public interface IShardingSet<TNode> : ISharding<TNode>
        where TNode : IShardingNode
    {
        /// <summary>
        /// 添加分区
        /// </summary>
        /// <param name="partition">分区</param>
        /// <returns>返回是否添加成功</returns>
        bool Add(IPartition<TNode> partition);

        /// <summary>
        /// 更新分区
        /// </summary>
        /// <param name="partition">分区</param>
        /// <returns>返回是否添加成功</returns>
        bool Update(IPartition<TNode> partition);

        /// <summary>
        /// 保存分区
        /// </summary>
        /// <param name="partition">分区</param>
        void Save(IPartition<TNode> partition);

        /// <summary>
        /// 批量加入节点
        /// </summary>
        /// <param name="partitions">节点列表</param>
        /// <returns>返回加入的节点</returns>
        List<IPartition<TNode>> AddAll(IList<IPartition<TNode>> partitions);

        /// <summary>
        /// 删除节点
        /// </summary>
        /// <param name="slot">移除槽位</param>
        /// <returns>返回移除节点关联的分区</returns>
        IPartition<TNode> Remove(long slot);

        /// <summary>
        /// 删除节点
        /// </summary>
        /// <param name="partition">删除的节点</param>
        /// <returns>返回移除节点关联的分区</returns>
        bool Remove(IPartition<TNode> partition);

        /// <summary>
        /// 清除
        /// </summary>
        void Clear();
    }

}
