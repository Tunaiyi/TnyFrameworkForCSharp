// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;

namespace TnyFramework.Codec.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
public class CodableAttribute : Attribute
{
    /// <summary>
    /// 协议 id
    /// </summary>
    public string Mime { get; }

    public CodableAttribute(string mime)
    {
        Mime = mime;
    }
}
