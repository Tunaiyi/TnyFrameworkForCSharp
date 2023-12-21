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
using TnyFramework.Net.Application;
using TnyFramework.Net.Endpoint;
using TnyFramework.Net.Exceptions;
using TnyFramework.Net.Message;

namespace TnyFramework.Net.Transport
{

    public abstract class BaseNetTunnel<TEndpoint, TTransporter> : NetTunnel<TEndpoint>
        where TEndpoint : INetEndpoint
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
                    OnDisconnect();
                }
                Status = TunnelStatus.Init;
            }
        }

        public override async ValueTask Write(IMessage message, bool waitWritten = false)
        {
            if (!await CheckAvailable(null))
            {
                return;
            }
            if (waitWritten)
            {
                await Transporter.Write(message);
            } else
            {
                _ = Transporter.Write(message);
            }
        }

        public override async ValueTask Write(MessageAllocator allocator, MessageContent messageContent, bool waitWritten = false)
        {
            if (!await CheckAvailable(null))
            {
                return;
            }
            if (waitWritten)
            {
                await Transporter.Write(allocator, messageContent);
            } else
            {
                _ = Transporter.Write(allocator, messageContent);
            }
        }

        protected virtual void OnWriteUnavailable()
        {

        }

        protected override void OnDisconnect()
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

        private ValueTask<bool> CheckAvailable(MessageContent? content)
        {
            if (IsActive())
            {
                return ValueTask.FromResult(true);
            }
            OnWriteUnavailable();
            var cause = new TunnelDisconnectedException($"{this} is disconnect");
            if (content != null)
            {
                content.Cancel(false);
            } else
            {
                return ValueTask.FromException<bool>(cause);
            }
            return ValueTask.FromResult(false);
        }

        public override string ToString()
        {
            return $"Tunnel({AccessMode})[{ContactGroup}({Identify})]{Transporter}";
        }
    }

}
