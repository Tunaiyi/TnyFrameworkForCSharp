using System.Collections.Generic;
using System.Threading.Tasks;
using TnyFramework.Namespace.Sharding;

namespace TnyFramework.Namespace
{

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

}
