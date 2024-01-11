// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Collections.Concurrent;
using TnyFramework.Common.Enum;

namespace TnyFramework.Net.Nats.Transports
{

    public class NatsAction : BaseEnum<NatsAction>
    {
        internal const int CONNECT_ID = 0;
        internal const int CONNECTED_ID = 1;
        internal const int CLOSE_ID = 2;
        internal const int PING_ID = 3;
        internal const int PONE_ID = 4;
        internal const int MESSAGE_ID = 6;

        public static readonly NatsAction CONNECT = Of(CONNECT_ID);
        public static readonly NatsAction CONNECTED = Of(CONNECTED_ID);
        public static readonly NatsAction CLOSE = Of(CLOSE_ID);
        public static readonly NatsAction PING = Of(PING_ID);
        public static readonly NatsAction PONE = Of(PONE_ID);
        public static readonly NatsAction MESSAGE = Of(MESSAGE_ID);

        private static readonly ConcurrentDictionary<string, NatsAction> ACTION_TYPE_MAP = new();

        public string Value { get; private init; } = "";

        private static NatsAction Of(int id)
        {
            return E(id, new NatsAction {
                Value = $"{id}",
            });
        }

        protected override void OnCheck()
        {
            if (ACTION_TYPE_MAP.TryAdd(Value, this))
                return;
            var value = ACTION_TYPE_MAP[Value];
            if (!ReferenceEquals(value, this))
            {
                throw new ArgumentException($"{value} 与 {this} 存在相同的 {nameof(Value)} {Value}");
            }
        }

        public static NatsAction OfValue(string action)
        {
            if (ACTION_TYPE_MAP.TryGetValue(action, out var value))
            {
                return value;
            }
            throw new ArgumentException($"未找到 {nameof(Value)} 为 {action} 的 {nameof(NatsAction)}");
        }
    }

}
