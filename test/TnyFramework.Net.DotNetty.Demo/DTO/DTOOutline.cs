// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Reflection;

namespace TnyFramework.Net.DotNetty.Demo.DTO;

public static partial class DTOOutline
{
    /// <summary>
    /// 注册所有DTO
    /// </summary>
    public static void RegisterDTOs()
    {
            var type = typeof(DTOOutline);
            var fields = type.GetFields(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            foreach (var filed in fields)
            {
                if (filed.FieldType != typeof(Action) || !filed.Name.StartsWith("Register", StringComparison.Ordinal))
                    continue;
                var action = (Action?) filed.GetValue(type);
                action?.Invoke();
            }
        }
}