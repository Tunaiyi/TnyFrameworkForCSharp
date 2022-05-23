namespace TnyFramework.Namespace.Etcd
{

    public class EtcdWatchOption
    {
        public string EndKey { get; set; }

        public long Revision { get; set; } = 0L;

        public bool PrevKv { get; set; } = false;

        public bool ProgressNotify { get; set; } = false;

        public bool Prefix { get; set; } = false;

        public bool NoPut { get; set; } = false;

        public bool NoDelete { get; set; } = false;

        public bool RequireLeader { get; set; } = false;
    }

}
