// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using TnyFramework.Common.Extensions;
using TnyFramework.Common.Logger;
using TnyFramework.Net.Message;
using TnyFramework.Net.Transport;

namespace TnyFramework.Net.Application
{

    public static class NetLogger
    {
        private static ILogger GetMessageSendLogger(string group, IMessageSchema message)
        {
            return NetLoggerGroup.OfRpc(group).ForSendMessage(message.Mode);
        }

        private static ILogger GetMessageReceiveLogger(string group, IMessageSchema message)
        {
            return NetLoggerGroup.OfRpc(group).ForReceiveMessage(message.Mode);
        }

        internal static void LogSend(ITunnel tunnel, IMessage message)
        {
            var logger = GetMessageSendLogger(tunnel.ContactGroup, message);
            logger?.LogDebug("# {tunnel} [发送] =>> Message : {message}", tunnel, message);
        }

        internal static void LogReceive(ITunnel tunnel, IMessage message)
        {
            var logger = GetMessageReceiveLogger(tunnel.ContactGroup, message);
            logger?.LogDebug("# {tunnel} [接收] <<= Message : {message}", tunnel, message);
        }
    }

    internal class NetLoggerGroup
    {
        private readonly ILogger[] receiveLoggers;

        private readonly ILogger[] sendLoggers;

        private static readonly Func<NetworkWay, string, string, ILogger> MESSAGE_RECEIVE_LOGGER_NAME_FACTORY =
            (way, group, mode) => LogFactory.Logger($"com.tny.game.net.rpc.{way.Value}.receive.{group.ToLower()}.{mode.ToLower()}");

        private static readonly Func<NetworkWay, string, string, ILogger> MESSAGE_SEND_LOGGER_NAME_FACTORY =
            (way, group, mode) => LogFactory.Logger($"com.tny.game.net.roc.{way.Value}.send.{group.ToLower()}.{mode.ToLower()}");

        private static readonly ConcurrentDictionary<string, NetLoggerGroup> NET_LOGGER_GROUP_MAP =
            new ConcurrentDictionary<string, NetLoggerGroup>();

        internal static NetLoggerGroup OfRpc(string userType)
        {
            var key = $"Rpc:{userType}";
            var logger = NET_LOGGER_GROUP_MAP.Get(key);
            if (logger != null)
            {
                return logger;
            }
            logger = new NetLoggerGroup(userType, MESSAGE_RECEIVE_LOGGER_NAME_FACTORY, MESSAGE_SEND_LOGGER_NAME_FACTORY);
            return NET_LOGGER_GROUP_MAP.GetOrAdd($"Rpc:{userType}", logger);
        }

        private NetLoggerGroup(string group,
            Func<NetworkWay, string, string, ILogger> receiveLoggerFactory,
            Func<NetworkWay, string, string, ILogger> sendLoggerFactory)
        {
            var enumSet = Enum.GetValues(typeof(MessageMode));
            receiveLoggers = new ILogger[enumSet.Length];
            sendLoggers = new ILogger[enumSet.Length];
            foreach (var value in enumSet)
            {
                if (value is not MessageMode mode)
                    continue;
                receiveLoggers[mode.GetIndex()] = receiveLoggerFactory(mode.GetWay(), group, $"{mode}");
                sendLoggers[mode.GetIndex()] = sendLoggerFactory(mode.GetWay(), group, $"{mode}");
            }
        }

        public ILogger ForReceiveMessage(MessageMode enumValue)
        {
            return receiveLoggers[enumValue.GetIndex()];
        }

        public ILogger ForSendMessage(MessageMode enumValue)
        {
            return sendLoggers[enumValue.GetIndex()];
        }
    }

}
