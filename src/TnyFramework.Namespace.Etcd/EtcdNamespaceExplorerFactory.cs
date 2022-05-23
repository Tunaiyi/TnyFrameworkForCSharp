using System.Collections.Generic;
using TnyFramework.Codec;

namespace TnyFramework.Namespace.Etcd
{

    public class EtcdNamespaceExplorerFactory : INamespaceExplorerFactory
    {
        private EtcdAccessor accessor;

        private readonly EtcdConfig config;

        private readonly ObjectCodecAdapter objectCodecAdapter;

        private volatile EtcdNamespaceExplorer namespaceExplorer;

        public EtcdNamespaceExplorerFactory(EtcdConfig config, ObjectCodecAdapter objectCodecAdapter)
        {
            this.config = config;
            this.objectCodecAdapter = objectCodecAdapter;
        }

        public INamespaceExplorer Create()
        {
            if (namespaceExplorer != null)
            {
                return namespaceExplorer;
            }
            lock (this)
            {
                if (namespaceExplorer != null)
                {
                    return namespaceExplorer;
                }
                accessor = new EtcdAccessor(config);
                accessor.Init().Wait();
                namespaceExplorer = new EtcdNamespaceExplorer(accessor, objectCodecAdapter);
            }
            return namespaceExplorer;
        }
    }

    public class EtcdConfig
    {
        public string Encoding { get; set; } = "utf-8";

        public string Endpoints { get; set; }

        public string User { get; set; }

        public string Password { get; set; }
    }

}
