// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;

namespace TnyFramework.Net.Message;

public static class Protocols
{
    public static readonly int PING_PONG_PROTOCOL_NUM = -1;

    public static IProtocol PUSH = DefaultProtocol.Protocol(0, 0);

    public static IProtocol Protocol(int protocol)
    {
        return DefaultProtocol.Protocol(protocol, 0);
    }

    public static IProtocol Protocol(int protocol, int line)
    {
        return DefaultProtocol.Protocol(protocol, line);
    }
}

/// <summary>
/// 协议接口
/// </summary>
public interface IProtocol
{
    /// <summary>
    /// 协议号
    /// </summary>
    int ProtocolId { get; }

    /// <summary>
    /// 获取信道号
    /// </summary>
    int Line { get; }

    /// <summary>
    /// 指定消息是否是属于此协议
    /// </summary>
    /// <param name="protocol"> 协议</param>
    /// <returns>是返回 true, 否则返回 false</returns>
    bool IsOwn(IProtocol protocol);
}

public abstract class BaseProtocol : IProtocol
{
    protected BaseProtocol(int protocolId, int line)
    {
        ProtocolId = protocolId;
        Line = line;
    }

    public int ProtocolId { get; }

    public int Line { get; }

    public bool IsOwn(IProtocol protocol)
    {
        throw new NotImplementedException();
    }
}

public sealed class DefaultProtocol : BaseProtocol
{
    private DefaultProtocol(int protocolId, int line) : base(protocolId, line)
    {
    }

    public static IProtocol Protocol(int protocol)
    {
        return new DefaultProtocol(protocol, 0);
    }

    public static IProtocol Protocol(int protocol, int line)
    {
        return new DefaultProtocol(protocol, line);
    }
}
