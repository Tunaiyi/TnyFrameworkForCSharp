// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using TnyFramework.Codec;
using TnyFramework.Namespace.Sharding;

namespace TnyFramework.Namespace.Etcd
{

    public class EtcdNodeHashingMultimapFactory : INodeHashingFactory
    {
        private static readonly EtcdNodeHashingMultimapFactory FACTORY = new EtcdNodeHashingMultimapFactory();

        public static INodeHashingFactory Default => FACTORY;

        public INodeHashing<TNode>? Create<TNode>(string rootPath, HashingOptions<TNode> option, INamespaceExplorer explorer,
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
