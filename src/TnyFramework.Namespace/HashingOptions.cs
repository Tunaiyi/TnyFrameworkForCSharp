using System.Text;

namespace TnyFramework.Namespace.Sharding
{

    public class HashingOptions<TNode>
        where TNode : IShardingNode
    {
        
        public string Name { get; set; }

        public long Ttl { get; set; } = 3000L;

        public int PartitionCount { get; set; } = 5;

        public long MaxSlots { get; set; } = 1024L;

        public Encoding Encoding { get; set; } = Encoding.UTF8;

        public IHasher<string> KeyHasher { get; set; }

        public IHasher<PartitionSlot<TNode>> NodeHasher { get; set; }

        public HashingOptions()
        {
        }
        
    }

}
