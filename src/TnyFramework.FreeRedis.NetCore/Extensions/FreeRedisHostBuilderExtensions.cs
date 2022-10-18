// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TnyFramework.Codec;
using TnyFramework.DI.Extensions;
using TnyFramework.DI.NetCore.Extensions;
using TnyFramework.FreeRedis.NetCore.Configurations;

namespace TnyFramework.FreeRedis.NetCore.Extensions
{

    public static class FreeRedisHostBuilderExtensions
    {
        public static IHostBuilder UseFreeRedis(this IHostBuilder builder, string section = FreeRedisPropertiesKeys.FREE_REDIS_ROOT)
        {
            return builder.ConfigureServices((context, services) => FreeRedisServices(context, services, section));
        }

        private static void FreeRedisServices(HostBuilderContext context, IServiceCollection services, string section)
        {
            services.BindProperties<FreeRedisProperties>(context, section);
            var descriptor = ServiceDescriptor.Singleton<ObjectCodecAdapter, ObjectCodecAdapter>();
            if (!services.Contains(descriptor))
            {
                services.BindSingleton<ObjectCodecAdapter>();
            }
            services.BindSingleton<FreeRedisDataSources>();
        }
    }

}
