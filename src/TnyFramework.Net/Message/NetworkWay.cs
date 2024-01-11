// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using TnyFramework.Common.Enum;

namespace TnyFramework.Net.Message;

public class NetworkWay : BaseEnum<NetworkWay>
{
    public static readonly NetworkWay MESSAGE = Of(0, "message");

    public static readonly NetworkWay SYSTEM = Of(1, "system");

    public static readonly NetworkWay HEARTBEAT = Of(2, "heartbeat");

    public string Value { get; private set; } = "";

    private static NetworkWay Of(int id, string value)
    {
        return E(id, new NetworkWay {
            Value = value,
        });
    }
}
