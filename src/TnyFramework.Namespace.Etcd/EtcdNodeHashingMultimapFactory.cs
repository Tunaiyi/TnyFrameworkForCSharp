using TnyFramework.Codec;
using TnyFramework.Namespace.Sharding;

namespace TnyFramework.Namespace.Etcd
{

    public class EtcdNodeHashingMultimapFactory : INodeHashingFactory
    {
        private static readonly EtcdNodeHashingMultimapFactory FACTORY = new EtcdNodeHashingMultimapFactory();

        public static INodeHashingFactory Default => FACTORY;

        public INodeHashing<TNode> Create<TNode>(string rootPath, HashingOptions<TNode> option, INamespaceExplorer explorer,
            ObjectCodecAdapter adapter) where TNode : IShardingNode
        {
            if (explorer is EtcdNamespaceExplorer etcdExplorer)
            {
                return new EtcdNodeHashingMultimap<TNode>(rootPath, option, etcdExplorer, adapter);
            }
            return null;
        }
    }

}
