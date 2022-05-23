using System.Collections.Generic;

namespace TnyFramework.Namespace.Sharding.Listener
{

    /// <summary>
    /// 增加分片改变
    /// </summary>
    /// <typeparam name="TNode">类型</typeparam>
    public delegate void ShardingOnChange<TNode>(ISharding<TNode> sharding, IReadOnlyCollection<IPartition<TNode>> partitions) where TNode : IShardingNode;

    /// <summary>
    /// 移除分片改变
    /// </summary>
    /// <typeparam name="TNode">类型</typeparam>
    public delegate void ShardingOnRemove<TNode>(ISharding<TNode> sharding, IReadOnlyCollection<IPartition<TNode>> partitions) where TNode : IShardingNode;

}
