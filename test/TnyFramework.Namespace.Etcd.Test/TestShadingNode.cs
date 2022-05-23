using TnyFramework.Namespace.Sharding;

namespace TnyFramework.Namespace.Etcd.Test
{

    public class TestShadingNode : IShardingNode
    {
        public string Key { get; set; }

        public TestShadingNode()
        {
        }

        public TestShadingNode(string key)
        {
            Key = key;
        }

        protected bool Equals(TestShadingNode other)
        {
            return Key == other.Key;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TestShadingNode) obj);
        }

        public override int GetHashCode()
        {
            return (Key != null ? Key.GetHashCode() : 0);
        }

        public override string ToString()
        {
            return $"{nameof(Key)}: {Key}";
        }
    }

}
