// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System.Net;
using Microsoft.AspNetCore.Hosting;

namespace TnyFramework.Net.Kestrel;

public static class WebHostBuilderExtension
{

    public static IWebHostBuilder UseCyberNet(this IWebHostBuilder builder, IPEndPoint address)
    {

        builder.ConfigureKestrel((context, kestrel) => {
            // var section = context.Configuration.GetSection("Kestrel");
            // kestrel.Configure(section).Endpoint(address, listenOptions =>
            // {
            //     // 设置feature，用于外围获取当前监听端口
            //     kestrel.ApplicationServices.GetRequiredService<IServer>().Features
            //         .Set<ICyberNetListenOptionsFeature>(new CyberNetListenOptionsFeature(listenOptions));
            //
            //     listenOptions.Run(connectionContext => OnConnectedAsync(connectionContext, listenOptions.ApplicationServices));
            // });
        });

        return builder;
    }
}
