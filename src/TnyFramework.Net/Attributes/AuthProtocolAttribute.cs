// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;

namespace TnyFramework.Net.Attributes;

/// <summary>
/// 配置生效协议号, 使用在 IAuthenticateValidator 实现类上. 自动关联对应协议
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class AuthProtocolAttribute : Attribute
{
    public AuthProtocolAttribute(params int[] protocols)
    {
        Protocols = protocols;
        All = false;
    }

    public AuthProtocolAttribute(bool all)
    {
        Protocols = new int[] { };
        All = all;
    }

    /// <summary>
    /// 协议号
    /// </summary>
    public int[] Protocols { get; }

    /// <summary>
    /// 是否是全部
    /// </summary>
    public bool All { get; }
};
