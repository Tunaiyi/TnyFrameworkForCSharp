// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using TnyFramework.Codec;

namespace TnyFramework.Namespace.Etcd;

public class EtcdNamespaceExplorerFactory : INamespaceExplorerFactory
{
    private EtcdAccessor? accessor;

    private readonly EtcdConfig config;

    private readonly ObjectCodecAdapter objectCodecAdapter;

    private volatile EtcdNamespaceExplorer? namespaceExplorer;

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

    public string Endpoints { get; set; } = "";

    public string User { get; set; } = "";

    public string Password { get; set; } = "";
}
