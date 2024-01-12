// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using TnyFramework.Net.DotNetty.Guide;
using TnyFramework.Net.DotNetty.Hosting.Guide;
using TnyFramework.Net.Hosting.Rpc;

namespace TnyFramework.Net.DotNetty.Hosting.Configuration;

public interface INettyRpcHostServerConfiguration
    : IRpcEndPointHostConfiguration<INettyServerGuide, INettyServerSetting, INettyServerGuideUnitContext, INettyRpcHostServerConfiguration, INettyServerGuideSpec>
{
    public INettyRpcHostServerConfiguration RpcServer(string name, string host, int port, bool libuv, Action<INettyServerGuideSpec>? action = null);

    public INettyRpcHostServerConfiguration RpcServer(string name, string serveName, string host, int port, bool libuv,
        Action<INettyServerGuideSpec>? action = null);
}
