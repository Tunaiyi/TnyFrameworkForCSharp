using TnyFramework.Codec;
using TnyFramework.Common.Utils;

namespace TnyFramework.Namespace.Etcd
{

    public abstract class EtcdHashing<TValue>
    {
        internal INamespaceExplorer Explorer { get; }

        public ObjectMimeType<TValue> MineType { get; }

        public string Path { get; }

        public long MaxSlots { get; }

        protected EtcdHashing(string path, long maxSlots, ObjectMimeType<TValue> mineType, INamespaceExplorer explorer)
        {
            Explorer = explorer;
            MaxSlots = maxSlots;
            Path = path;
            MineType = mineType;
        }

        internal string SubPath(long slot)
        {
            return NamespacePathNames.NodePath(Path, SlotName(slot));
        }
        
        internal string SlotName(long hashCode)
        {
            if (hashCode >= MaxSlots)
            {
                hashCode %= MaxSlots;
            }
            return NumberFormatAide.AlignDigits(hashCode, MaxSlots);
        }
    }

}
