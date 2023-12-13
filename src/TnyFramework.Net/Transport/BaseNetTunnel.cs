// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TnyFramework.Common.Extensions;
using TnyFramework.Common.Logger;
using TnyFramework.Net.Base;
using TnyFramework.Net.Endpoint;
using TnyFramework.Net.Exceptions;
using TnyFramework.Net.Message;

namespace TnyFramework.Net.Transport
{

    public abstract class BaseNetTunnel<TUserId, TEndpoint, TTransporter> : NetTunnel<TUserId, TEndpoint>
        where TEndpoint : INetEndpoint<TUserId>
        where TTransporter : IMessageTransporter
    {
        protected TTransporter Transporter { get; }

        public override EndPoint? RemoteAddress => Transporter.RemoteAddress;

        public override EndPoint? LocalAddress => Transporter.LocalAddress;

        protected readonly ILogger logger;

        protected BaseNetTunnel(long id, TTransporter transporter, NetAccessMode accessMode, INetworkContext context)
            : base(id, accessMode, context)
        {
            logger = LogFactory.Logger(GetType());
            if (transporter.IsNotNull())
            {
                Transporter = transporter;
                Transporter.Bind(this);
            } else
            {
                Transporter = default!;
            }
        }

        public override bool IsActive()
        {
            var transporter = Transporter;
            return Status == TunnelStatus.Open && transporter.IsNotNull() && transporter.IsActive();
        }

        public override void Reset()
        {

            if (Status == TunnelStatus.Init)
            {
                return;
            }
            lock (this)
            {
                if (Status == TunnelStatus.Init)
                {
                    return;
                }
                if (!IsActive())
                {
                    DoDisconnect();
                }
                Status = TunnelStatus.Init;
            }
        }

        public override Task Write(IMessage message)
        {
            return CheckAvailable(null, out var task) ? Transporter.Write(message) : task;
        }

        public override Task Write(MessageAllocator allocator, MessageContent messageContent)
        {
            return CheckAvailable(messageContent, out var task) ? Transporter.Write(allocator, MessageFactory, messageContent) : task;
        }

        protected virtual void OnWriteUnavailable()
        {

        }

        protected override void DoDisconnect()
        {
            var transporter = Transporter;
            if (transporter.IsNull() || !transporter.IsActive())
                return;
            try
            {
                transporter.Close();
            } catch (Exception e)
            {
                logger.LogError(e, "transporter close error");
            }
        }

        private bool CheckAvailable(MessageContent? content, out Task task)
        {
            if (IsActive())
            {
                task = null!;
                return true;
            }
            OnWriteUnavailable();
            if (content != null)
            {
                content.Cancel(false);
                task = content.Written();
            } else
            {
                var cause = new TunnelDisconnectedException($"{this} is disconnect");
                task = Task.FromException(cause);
            }
            return false;
        }

        public override string ToString()
        {
            return $"Tunnel({AccessMode})[{UserGroup}({UserId})]{Transporter}";
        }
    }

}
