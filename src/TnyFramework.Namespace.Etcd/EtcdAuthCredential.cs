// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System.Threading.Tasks;
using dotnet_etcd;
using Etcdserverpb;
using Grpc.Core;
using TnyFramework.Common.Extensions;

namespace TnyFramework.Namespace.Etcd;

public class EtcdAuthCredential
{
    private readonly EtcdClient client;

    private readonly EtcdConfig config;

    private volatile Metadata.Entry? tokenEntry;

    public bool NeedAuthenticate { get; }

    public EtcdAuthCredential(EtcdClient client, EtcdConfig config)
    {
        this.client = client;
        this.config = config;
        NeedAuthenticate = config.User.IsNotBlank() && config.Password.IsNotBlank();
    }

    public async Task<Metadata.Entry?> ApplyToken()
    {
        if (!NeedAuthenticate)
        {
            return null;
        }
        if (tokenEntry != null)
        {
            return tokenEntry;
        }
        await DoAuthenticate();
        return tokenEntry;
    }

    public void Refresh()
    {
        tokenEntry = null;
    }

    private async Task DoAuthenticate()
    {
        if (config.User.IsNotBlank() && config.Password.IsNotBlank())
        {
            var authRes = await client.AuthenticateAsync(new AuthenticateRequest {
                Name = config.User,
                Password = config.Password
            });
            tokenEntry = new Metadata.Entry("token", authRes.Token);
        }
    }
}
