// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using Microsoft.Extensions.Logging;
using TnyFramework.Common.Logger;
using TnyFramework.Net.Message;
using TnyFramework.Net.Plugin;
using TnyFramework.Net.Transport;

namespace TnyFramework.Net.Command.Dispatcher
{

    internal class PluginChain
    {
        private static readonly ILogger LOGGER = LogFactory.Logger<PluginChain>();

        private PluginChain? next;

        private readonly CommandPluginHolder? plugin;

        public PluginChain(CommandPluginHolder plugin)
        {
            this.plugin = plugin;
        }

        public void Execute(ITunnel tunnel, IMessage message, RpcInvokeContext context)
        {
            if (plugin == null || context.Done)
            {
                return;
            }
            try
            {
                plugin.InvokePlugin(tunnel, message, context);
            } catch (Exception e)
            {
                LOGGER.LogError(e, "invoke plugin {Plugin} exception", plugin.GetType());
            }
            if (next == null || context.Done)
            {
                return;
            }
            next.Execute(tunnel, message, context);

        }

        internal void Append(PluginChain chain)
        {
            if (next?.plugin == null)
            {
                next = chain;
            } else
            {
                next.Append(chain);
            }
        }
    }

}
