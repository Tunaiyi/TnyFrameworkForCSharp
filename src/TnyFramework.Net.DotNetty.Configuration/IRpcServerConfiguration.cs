// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using TnyFramework.DI.Units;
using TnyFramework.Net.Base;
using TnyFramework.Net.DotNetty.Configuration.Guide;
using TnyFramework.Net.DotNetty.Configuration.Rpc;
using TnyFramework.Net.Rpc;
using TnyFramework.Net.Transport;

namespace TnyFramework.Net.DotNetty.Configuration
{

    public interface IRpcServerConfiguration : INettyServerConfiguration
    {
        RpcServerConfiguration RpcServer(ServerSetting setting, Action<INetServerGuideSpec<RpcAccessIdentify>>? action = null);

        RpcServerConfiguration RpcServer(string name, int port, Action<INetServerGuideSpec<RpcAccessIdentify>>? action = null);

        RpcServerConfiguration RpcServer(string name, string host, int port, Action<INetServerGuideSpec<RpcAccessIdentify>>? action = null);

        RpcServerConfiguration RpcServer(string name, string host, int port, bool libuv,
            Action<INetServerGuideSpec<RpcAccessIdentify>>? action = null);

        RpcServerConfiguration RpcServer(string name, string serveName, string host, int port,
            Action<INetServerGuideSpec<RpcAccessIdentify>>? action = null);

        RpcServerConfiguration RpcServer(string name, string serveName, string host, int port, bool libuv,
            Action<INetServerGuideSpec<RpcAccessIdentify>>? action = null);

        RpcServerConfiguration RpcAuthServiceSpecConfigure(Action<RpcAuthServiceSpec> action);

        RpcServerConfiguration IdGeneratorConfigure(Action<UnitSpec<IIdGenerator, IRpcUnitContext>> action);
    }

}
