// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TnyFramework.Common.Attribute;
using TnyFramework.Common.Logger;
using TnyFramework.Net.Application;
using TnyFramework.Net.Transport;

namespace TnyFramework.Net.Nats.Transports
{

    public class NatsClient : AttributesContext, IClient
    {
        private ITunnel? tunnel;

        private int status;

        private readonly ILogger logger = LogFactory.Logger<NatsClient>();

        private readonly Uri uri;
        private readonly NatsChannel channel;
        private readonly ConnectedHandle? handle;
        private readonly bool reconnect;

        public NatsClient(Uri uri, NatsChannel channel, ConnectedHandle? handle, bool reconnect = false)
        {
            this.uri = uri;
            this.channel = channel;
            this.handle = handle;
            this.reconnect = reconnect;

        }

        public Uri Url() => uri;

        public async ValueTask<bool> Open()
        {
            if (Interlocked.CompareExchange(ref status, 1, 0) != 0)
            {
                return false;
            }
            var result = false;
            try
            {
                var connect = await channel.Connect(uri, handle);
                if (connect == null)
                {
                    return result;
                }
                tunnel = connect;
                result = true;
                return result;
            } catch (Exception e)
            {
                logger.LogError(e, "");
                throw;
            } finally
            {
                if (!result)
                {
                    Interlocked.Exchange(ref status, 0);
                }
            }
        }

        public async ValueTask<bool> Reconnect()
        {
            if (tunnel == null || !reconnect || tunnel.IsActive())
            {
                return false;
            }
            if (Interlocked.CompareExchange(ref status, 2, 1) != 1)
            {
                return false;
            }
            var result = false;
            try
            {
                var connect = await channel.Connect(uri, handle);
                if (connect == null)
                {
                    return result;
                }
                tunnel = connect;
                Interlocked.Exchange(ref status, 1);
                result = true;
                return result;
            } catch (Exception e)
            {
                logger.LogError(e, "");
                throw;
            } finally
            {
                if (!result)
                {
                    Interlocked.Exchange(ref status, 0);
                }
            }
        }

        public EndPoint? RemoteAddress => tunnel?.RemoteAddress;

        public EndPoint? LocalAddress => tunnel?.LocalAddress;

        public bool IsActive()
        {
            return tunnel?.IsActive() ?? false;
        }

        public bool IsClosed()
        {
            return tunnel?.IsClosed() ?? false;
        }

        public bool Close()
        {
            Interlocked.Exchange(ref status, 3);
            tunnel?.Close();
            return true;
        }
    }

}
