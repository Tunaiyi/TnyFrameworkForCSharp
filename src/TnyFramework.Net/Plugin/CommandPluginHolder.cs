using Microsoft.Extensions.Logging;
using TnyFramework.Common.Logger;
using TnyFramework.Net.Attributes;
using TnyFramework.Net.Dispatcher;
using TnyFramework.Net.Message;
using TnyFramework.Net.Transport;

namespace TnyFramework.Net.Plugin
{

    public class CommandPluginHolder
    {
        private static readonly ILogger LOGGER = LogFactory.Logger<CommandPluginHolder>();

        private readonly ICommandPlugin plugin;

        private readonly ControllerHolder controller;

        private readonly object attributes;

        public CommandPluginHolder(ControllerHolder controller, ICommandPlugin plugin, PluginAttribute attributes)
            : this(controller, plugin, attributes.Attribute)
        {
        }

        private CommandPluginHolder(ControllerHolder controller, ICommandPlugin plugin, object attributes)
        {
            this.plugin = plugin;
            this.controller = controller;
            this.attributes = attributes;
        }

        public void InvokePlugin(ITunnel tunnel, IMessage message, MessageCommandContext context)
        {
            if (LOGGER.IsEnabled(LogLevel.Debug))
            {
                LOGGER.LogDebug("调用 {Type}.{Handle} | 触发插件 {Plugin}", controller.ControllerType, controller.Name, plugin.GetType());
            }
            plugin.Execute(tunnel, message, context, attributes);
        }
    }

}
