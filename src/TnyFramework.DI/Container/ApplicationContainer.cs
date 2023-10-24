// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using TnyFramework.Common.Exceptions;

namespace TnyFramework.DI.Container
{

    /// <summary>
    /// 应用容器
    /// </summary>
    public class ApplicationContainer
    {
        private const int STATUS_INIT = 0;
        private const int STATUS_START = 0;
        private const int STATUS_STOP = 0;

        private volatile int status = STATUS_INIT;

        private readonly List<IApplicationModule> modules = new List<IApplicationModule>();

        private readonly ServiceCollection serviceCollection = new ServiceCollection();

        /// <summary>
        /// 获取服务提供器
        /// </summary>
        public IServiceProvider ServiceProvider { get; private set; } = null!;

        /// <summary>
        /// 注册模块
        /// </summary>
        /// <typeparam name="TModule">模块类型</typeparam>
        /// <returns>返回当前容器</returns>
        public ApplicationContainer Register<TModule>()
            where TModule : IApplicationModule, new()
        {
            if (status != STATUS_INIT)
                throw new CommonException($"ApplicationContainer 已启动");
            var module = Activator.CreateInstance<TModule>();
            modules.Add(module);
            return this;
        }

        /// <summary>
        /// 注册模块
        /// </summary>
        /// <param name="moduleInstance">模块对象</param>
        /// <returns>返回当前容器</returns>
        public ApplicationContainer Register(IApplicationModule moduleInstance)
        {
            if (status != STATUS_INIT)
                throw new CommonException($"ApplicationContainer 已启动");
            modules.Add(moduleInstance);
            return this;
        }

        /// <summary>
        /// 注册模块
        /// </summary>
        /// <param name="type">模块类型</param>
        /// <returns>返回当前容器</returns>
        /// <exception cref="CommonException"></exception>
        public ApplicationContainer Register(Type type)
        {
            if (status != STATUS_INIT)
                throw new CommonException($"ApplicationContainer 已启动");
            if (!typeof(IApplicationModule).IsAssignableFrom(type))
                throw new CommonException($"{type} 没有实现 {typeof(IApplicationModule)} 接口");
            var module = Activator.CreateInstance(type);
            if (module is IApplicationModule applicationModule)
            {
                modules.Add(applicationModule);
            }
            return this;
        }

        /// <summary>
        /// 启动
        /// </summary>
        public void Start()
        {
            var current = status;
            if (current != STATUS_INIT || Interlocked.CompareExchange(ref status, STATUS_START, current) != current)
                return;
            foreach (var appModule in modules)
            {
                appModule.Initialize(serviceCollection);
            }
            ServiceProvider = serviceCollection.BuildServiceProvider(new ServiceProviderOptions {
                ValidateOnBuild = true
            });
        }

        /// <summary>
        /// 关闭
        /// </summary>
        public void Close()
        {
            var current = status;
            if (current == STATUS_STOP || Interlocked.CompareExchange(ref status, STATUS_STOP, current) != current)
                return;
            foreach (var appModule in modules)
            {
                appModule.Close(serviceCollection);
            }
        }
    }

}
