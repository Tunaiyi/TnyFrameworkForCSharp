// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using Microsoft.Extensions.Logging;
using TnyFramework.Common.Extensions;
using TnyFramework.Net.Application;
using TnyFramework.Net.Session;

namespace TnyFramework.Net.Transport;

public class ServerTunnel<TTransporter> : BaseNetTunnel<INetSession, TTransporter>
    where TTransporter : IMessageTransporter
{
    public ServerTunnel(long id, TTransporter transporter, INetworkContext context, INetService service)
        : base(id, transporter, NetAccessMode.Server, context, service)
    {
    }

    protected override void OnDisconnect()
    {
        Close();
    }

    protected override bool OnOpen()
    {
        var transporter = Transporter;
        if (transporter.IsNotNull() && transporter.IsActive())
            return true;
        logger.LogWarning("open failed. channel {Transporter} is not active", transporter);
        return false;
    }
}
