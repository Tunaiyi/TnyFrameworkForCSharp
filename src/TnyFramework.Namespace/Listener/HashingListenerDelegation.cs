using System.Collections.Generic;
using TnyFramework.Namespace.Sharding;

namespace TnyFramework.Namespace.Listener
{

    public delegate void ShardingOnChange<TNode>(ISharding<TNode> source, List<IPartition<TNode>> partitions) where TNode : IShardingNode;

    public delegate void ShardingOnRemove<TNode>(ISharding<TNode> source, List<IPartition<TNode>> partitions) where TNode : IShardingNode;

}
