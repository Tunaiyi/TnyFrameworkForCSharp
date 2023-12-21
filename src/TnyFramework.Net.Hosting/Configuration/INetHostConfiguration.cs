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
using TnyFramework.Net.Command.Tasks;
using TnyFramework.Net.Hosting.App;
using TnyFramework.Net.Hosting.Endpoint;
using TnyFramework.Net.Plugin;

namespace TnyFramework.Net.Hosting.Configuration
{

    public interface INetHostConfiguration<out TConfiguration>
        where TConfiguration : INetHostConfiguration<TConfiguration>
    {
        TConfiguration AppContext(int serverId, string name);

        TConfiguration AppContext(int serverId, string name, string appType, string scope);

        TConfiguration AppContextConfigure(Action<INetAppContextSpec> action);

        TConfiguration MessageDispatcherConfigure(Action<IUnitSpec<IMessageDispatcher, INetUnitContext>> action);

        TConfiguration CommandBoxFactory(Action<IUnitSpec<ICommandBoxFactory, INetUnitContext>> action);

        TConfiguration AddController<TController>() where TController : class, IController;

        TConfiguration AddController(IController controller);

        TConfiguration AddController<TController>(Func<IServiceProvider, TController> factory) where TController : IController;

        TConfiguration AddControllers();

        TConfiguration AddControllers(ICollection<Assembly> assemblies);

        TConfiguration CommandPluginsConfigure(Action<IUnitCollectionSpec<ICommandPlugin, INetUnitContext>> action);

        TConfiguration EndpointConfigure(Action<IEndpointSpec> action);

        TConfiguration AuthenticateValidatorsConfigure(Action<IUnitCollectionSpec<IAuthenticationValidator, INetUnitContext>> action);

        TConfiguration AddAuthenticateValidators(Action<IUnitSpec<IAuthenticationValidator, INetUnitContext>> action);

        TConfiguration Initialize();
    }

}
