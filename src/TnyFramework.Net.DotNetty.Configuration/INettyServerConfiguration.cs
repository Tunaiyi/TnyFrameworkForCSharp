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
using TnyFramework.DI.Units;
using TnyFramework.Net.Command.Auth;
using TnyFramework.Net.Command.Dispatcher;
using TnyFramework.Net.Command.Processor;
using TnyFramework.Net.DotNetty.Configuration.Endpoint;
using TnyFramework.Net.DotNetty.Configuration.Guide;
using TnyFramework.Net.Plugin;
using TnyFramework.Net.Rpc;

namespace TnyFramework.Net.DotNetty.Configuration
{

    public interface INettyServerConfiguration
    {
        NettyServerConfiguration Server<TUserId>(string name, Action<INetServerGuideSpec<TUserId>> action);

        NettyServerConfiguration AppContext(int serverId, string name);

        NettyServerConfiguration AppContext(int serverId, string name, string appType, string scope);

        NettyServerConfiguration AppContextConfigure(Action<INetAppContextSpec> action);

        NettyServerConfiguration MessageDispatcherConfigure(Action<IUnitSpec<IMessageDispatcher, INetUnitContext>> action);

        NettyServerConfiguration CommandTaskBoxProcessor(Action<IUnitSpec<ICommandTaskBoxProcessor, INetUnitContext>> action);

        NettyServerConfiguration AddController<TController>() where TController : class, IController;

        NettyServerConfiguration AddController(IController controller);

        NettyServerConfiguration AddController<TController>(Func<IServiceProvider, TController> factory) where TController : IController;

        NettyServerConfiguration AddControllers();

        NettyServerConfiguration AddControllers(ICollection<Assembly> assemblies);

        NettyServerConfiguration CommandPluginsConfigure(Action<IUnitCollectionSpec<ICommandPlugin, INetUnitContext>> action);

        NettyServerConfiguration EndpointConfigure(Action<IEndpointSpec> action);

        NettyServerConfiguration AuthenticateValidatorsConfigure(Action<IUnitCollectionSpec<IAuthenticationValidator, INetUnitContext>> action);

        NettyServerConfiguration AddAuthenticateValidators(Action<IUnitSpec<IAuthenticationValidator, INetUnitContext>> action);
    }

}
