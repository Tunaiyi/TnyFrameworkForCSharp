// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;

namespace TnyFramework.Net.Attributes;

[AttributeUsage(AttributeTargets.Parameter)]
public class RpcParamAttribute : Attribute
{
    public RpcParamAttribute(int index = -1)
    {
        Index = index;
    }

    /// <summary>
    /// Index 值, 与 Key 互斥, 默认 -1, 按 MsgParam 顺序
    /// </summary>
    public int Index { get; } = -1;
}
