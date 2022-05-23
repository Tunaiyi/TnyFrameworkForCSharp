using TnyFramework.Codec;
using TnyFramework.Namespace.Sharding;

namespace TnyFramework.Namespace
{

    public interface INodeHashingFactory
    {
        INodeHashing<TNode> Create<TNode>(string rootPath, HashingOptions<TNode> option, INamespaceExplorer explorer, ObjectCodecAdapter adapter)
            where TNode : IShardingNode;
    }

}
