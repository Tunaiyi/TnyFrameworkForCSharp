// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http.Features;
using TnyFramework.Net.Base;
using TnyFramework.Net.DotNetty.Bootstrap;

namespace TnyFramework.Net.DotNetty.NetCore
{

    public class NetServer : IServer
    {
        private readonly INetServerGuide netServerGuide;

        public NetServer(INetServerGuide netServerGuide, IFeatureCollection features)
        {
            this.netServerGuide = netServerGuide;
            Features = features;
        }

        public void Dispose()
        {
        }

        public async Task StartAsync<TContext>(IHttpApplication<TContext> application, CancellationToken cancellationToken = default)
        {
            await netServerGuide.Open();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await netServerGuide.Close();
        }

        public IFeatureCollection Features { get; }
    }

}
