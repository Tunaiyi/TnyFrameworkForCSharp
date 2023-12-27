// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TnyFramework.Common.Extensions;
using TnyFramework.Common.Logger;
using TnyFramework.Net.Application;
using TnyFramework.Net.Command.Dispatcher;
using TnyFramework.Net.Rpc.Auth;
using TnyFramework.Net.Rpc.Configuration;
using TnyFramework.Net.Transport;

namespace TnyFramework.Net.Rpc.Client;

public class RpcClientFactory : IRpcClientFactory
{
    private readonly ILogger logger = LogFactory.Logger<RpcClientFactory>();
    private readonly IRpcRemoteServiceSetting setting;
    private readonly INetApplicationOptions application;
    private readonly IClientGuide guide;

    public RpcClientFactory(IRpcRemoteServiceSetting setting, INetApplicationOptions application, IClientGuide guide)
    {
        this.setting = setting;
        this.application = application;
        this.guide = guide;
    }

    public async ValueTask<IClient?> Create(Uri uri, int index)
    {
        var client = await guide.Client(uri, CreateConnectedHandle(index));
        try
        {
            if (!await client.Open())
            {
                return null;
            }
            logger.LogInformation("Rpc [{service}] Client {uri} connect success", guide.Service, uri);
            return client;
        } catch (Exception e)
        {
            logger.LogError(e, "Rpc [{service}] Client {uri} connect failed", guide.Service, uri);
            client.Close();
            throw;
        }
    }

    private ConnectedHandle CreateConnectedHandle(int index)
    {
        return tunnel => ConnectedHandle(tunnel, index);
    }

    private async ValueTask<bool> ConnectedHandle(INetTunnel tunnel, int index)
    {
        var username = setting.Username.IsNotBlank() ? setting.Username : guide.Service;
        var serviceType = RpcServiceType.ForService(username);
        var serverId = application.ServerId;
        var id = RpcAccessIdentify.FormatId(serviceType, serverId, index);
        var content = RpcAuthMessageContexts
            .AuthRequest(id, setting.Password)
            .WillRespondAwaiter(setting.AuthenticateTimeout);
        var networkContext = tunnel.Context;
        var invokeContext =
            RpcMessageTransactionContext.CreateExit(tunnel.Endpoint, content, networkContext.RpcMonitor, false);
        invokeContext.Invoke(
            RpcMessageTransactionContext.RpcOperation(nameof(RpcAuthController.Authenticate), content));
        try
        {
            var sent = await tunnel.Send(content);
            if (sent.Respond(out var task))
            {
                var message = await task;
                invokeContext.Complete(message);
                return message.ResultCode.IsSuccess();
            }
        } catch (Exception error)
        {
            invokeContext.Complete(error);
            throw;
        }
        return true;
    }
}
