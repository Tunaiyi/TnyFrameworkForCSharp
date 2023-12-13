// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using TnyFramework.Net.Message;

namespace TnyFramework.Net.Endpoint;

public interface IConnectIdentity : IContact
{
    /// <summary>
    /// 会话身份唯一标识
    /// 与 ContactId 可能不相同
    /// </summary>
    long Identify { get; }

    /// <summary>
    /// 获取用户 id
    /// </summary>
    object? IdentifyToken { get; }

    public bool HasIdentifyToken() => IdentifyToken != null;

    public T? GetIdentifyToken<T>()
    {
        var current = IdentifyToken;
        if (current is T value)
        {
            return value;
        }
        return default;
    }

    public bool GetIdentifyToken<T>(out T token)
    {
        var current = IdentifyToken;
        if (current is T value)
        {
            token = value;
            return true;
        }
        token = default!;
        return false;
    }
}
