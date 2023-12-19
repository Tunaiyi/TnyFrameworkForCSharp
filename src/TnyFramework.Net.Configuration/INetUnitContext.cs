
// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System.Collections.Generic;
using TnyFramework.Net.Base;
using TnyFramework.Net.Command.Auth;
using TnyFramework.Net.Command.Dispatcher;
using TnyFramework.Net.Command.Tasks;
using TnyFramework.Net.Configuration.Endpoint;
using TnyFramework.Net.Plugin;
using TnyFramework.Net.Rpc;

namespace TnyFramework.Net.Configuration
{

    public interface INetUnitContext
    {
        void Load();

        MessageDispatcherContext LoadMessageDispatcherContext();

        IMessageDispatcher LoadMessageDispatcher();

        ICommandBoxFactory LoadCommandBoxFactory();

        IEndpointUnitContext EndpointUnitContext { get; }

        INetAppContext LoadAppContext();

        IRpcInvokeNodeManager LoadRpcRemoteServiceManager();

        IList<ICommandPlugin> LoadCommandPlugins();

        IList<IAuthenticationValidator> LoadAuthenticateValidators();
    }

}
