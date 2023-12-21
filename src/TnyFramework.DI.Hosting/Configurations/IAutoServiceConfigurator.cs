// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace TnyFramework.DI.Hosting.Configurations
{

    public interface IAutoServiceConfigurator
    {
        /// <summary>
        /// 配置 service
        /// </summary>
        /// <param name="context">主机构建上下文</param>
        /// <param name="serviceCollection">服务集合</param>
        void Configure(HostBuilderContext context, IServiceCollection serviceCollection);
    }

}
