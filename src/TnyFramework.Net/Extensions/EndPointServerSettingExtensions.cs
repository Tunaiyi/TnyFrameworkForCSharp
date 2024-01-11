// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using TnyFramework.Common.Extensions;
using TnyFramework.Net.Application;

namespace TnyFramework.Net.Extensions;

public static class EndPointServerSettingExtensions
{
    public static bool IsHasEndPoint(this IServerSetting setting)
    {
        return setting.Host.IsNotBlank() && setting.Port > 0;
    }

    public static Uri GetUri(this IServerSetting setting)
    {
        return new Uri(setting.Url());
    }

    public static string Url(this IServerSetting setting)
    {
        return $"{setting.Scheme}://{setting.Host}:{setting.Port}";
    }
}
