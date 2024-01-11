// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace TnyFramework.Net.Application;

/// <summary>
/// 默认消息者类型
/// </summary>
public class NetContactType : ContactType<NetContactType>
{
    /// <summary>
    /// 匿名
    /// </summary>
    public static readonly NetContactType ANONYMITY = Of(0, ANONYMITY_USER_TYPE);

    /// <summary>
    /// 默认用户
    /// </summary>
    /// <returns></returns>
    public static readonly NetContactType DEFAULT_USER = Of(1, DEFAULT_USER_TYPE);
}
