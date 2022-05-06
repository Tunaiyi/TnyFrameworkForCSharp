using System;
using Microsoft.Extensions.Logging;
using TnyFramework.Common.Logger;
using TnyFramework.Net.Message;
using TnyFramework.Net.Plugin;
using TnyFramework.Net.Transport;
namespace TnyFramework.Net.Dispatcher
{
    internal class PluginChain
    {
        private static readonly ILogger LOGGER = LogFactory.Logger<PluginChain>();

        private PluginChain next;

        private readonly CommandPluginHolder plugin;


        public PluginChain(CommandPluginHolder plugin)
        {
            this.plugin = plugin;
        }


        public void Execute(ITunnel tunnel, IMessage message, MessageCommandContext context)
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
