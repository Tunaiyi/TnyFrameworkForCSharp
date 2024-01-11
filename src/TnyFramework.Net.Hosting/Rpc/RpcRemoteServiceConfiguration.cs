// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TnyFramework.Common.Exceptions;
using TnyFramework.Common.Logger;
using TnyFramework.DI.Extensions;
using TnyFramework.DI.Units;
using TnyFramework.Net.Hosting.Selectors;
using TnyFramework.Net.Rpc.Attributes;
using TnyFramework.Net.Rpc.Remote;

namespace TnyFramework.Net.Hosting.Rpc;

public class RpcRemoteServiceConfiguration : IRpcRemoteServiceConfiguration
{
    private static readonly ILogger LOGGER = LogFactory.Logger<RpcRemoteServiceConfiguration>();

    private bool autoRemoteServices;

    private bool initialized;

    private readonly ISet<Type> rpcRemoteServiceTypes = new HashSet<Type>();

    private RpcRemoteUnitContext RemoteUnitContext { get; }

    private IServiceCollection UnitContainer { get; }

    public static RpcRemoteServiceConfiguration CreateRpcRemoteService(IServiceCollection unitContainer)
    {
        return new RpcRemoteServiceConfiguration(unitContainer);
    }

    private RpcRemoteServiceConfiguration(IServiceCollection unitContainer)
    {
        UnitContainer = unitContainer;
        RemoteUnitContext = new RpcRemoteUnitContext(unitContainer);
        UnitContainer = unitContainer;
    }

    public RpcRemoteServiceConfiguration RpcRemoteSettingConfigure(
        Action<IUnitSpec<RpcRemoteSetting, IRpcRemoteUnitContext>> action)
    {
        action.Invoke(RemoteUnitContext.RpcRemoteSettingSpec);
        return this;
    }

    public RpcRemoteServiceConfiguration AddRemoteService<TRpcRemoteService>() where TRpcRemoteService : class
    {
        CheckRemoteServiceInterface<TRpcRemoteService>();
        rpcRemoteServiceTypes.Add(typeof(TRpcRemoteService));
        LOGGER.LogInformation("AddRemoteService : {RemoteService}", typeof(TRpcRemoteService));
        return this;
    }

    public RpcRemoteServiceConfiguration AddRemoteService(Type type)
    {
        CheckRemoteServiceInterface(type);
        rpcRemoteServiceTypes.Add(type);
        LOGGER.LogInformation("AddRemoteService : {RemoteService}", type);
        return this;
    }

    public RpcRemoteServiceConfiguration AddRemoteServices()
    {
        autoRemoteServices = true;
        return this;
    }

    private void CheckRemoteServiceInterface<T>()
    {
        CheckRemoteServiceInterface(typeof(T));
    }

    private bool IsRemoteServiceInterface(Type type)
    {
        return (type.IsInterface || type.IsAbstract) && type.GetCustomAttribute<RpcRemoteServiceAttribute>() != null;
    }

    private void CheckRemoteServiceInterface(Type type)
    {
        if (!(type.IsInterface || type.IsAbstract))
        {
            throw new IllegalArgumentException($"{type} 非接口");
        }
        if (type.GetCustomAttribute<RpcRemoteServiceAttribute>() == null)
        {
            throw new IllegalArgumentException($"{type} 未配置 {typeof(RpcRemoteServiceAttribute)} 特性");
        }
    }

    public RpcRemoteServiceConfiguration AddRemoteServices(IEnumerable<Type> types)
    {
        foreach (var type in types)
        {
            CheckRemoteServiceInterface(type);
            rpcRemoteServiceTypes.Add(type);
        }
        return this;
    }

    public RpcRemoteServiceConfiguration AddRemoteServices(ICollection<Assembly> assemblies)
    {
        foreach (var assembly in assemblies)
        {
            foreach (var type in assembly.GetTypes())
            {
                if (IsRemoteServiceInterface(type))
                {
                    rpcRemoteServiceTypes.Add(type);
                }
            }
        }
        return this;
    }

    private Func<IServiceProvider, object> ServiceFunc(Type type)
    {
        return provider => {
            var factory = provider.GetService<RpcRemoteInstanceFactory>();
            if (factory != null)
                return factory.Create(type);
            throw new NullReferenceException($"{typeof(RpcRemoteInstanceFactory)} is null");
        };
    }

    public RpcRemoteServiceConfiguration Initialize()
    {
        if (initialized)
            return this;
        RemoteUnitContext.Load();
        var types = new HashSet<Type>();
        if (autoRemoteServices)
        {
            foreach (var type in RpcTypeSelector.RemoteService)
            {
                if (types.Add(type))
                {
                    UnitContainer.BindSingleton(type, ServiceFunc(type));
                }
            }
        }
        foreach (var type in rpcRemoteServiceTypes)
        {
            if (types.Add(type))
            {
                UnitContainer.BindSingleton(type, ServiceFunc(type));
            }
        }
        initialized = true;
        return this;
    }
}
