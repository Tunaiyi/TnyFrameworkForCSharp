// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

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
