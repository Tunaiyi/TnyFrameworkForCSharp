// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using TnyFramework.Common.Logger;

namespace TnyFramework.Extensions.Configuration
{

    public static class TnyEnvironments
    {
        private static readonly ILogger LOGGER = LogFactory.Logger(typeof(TnyEnvironments));
        public const string APP_NAME = "APP_NAME";
        public const string SERVER_ID = "SERVER_ID";
        public const string APP_INSTANCE = "APP_INSTANCE";
        public const string APP_TYPE = "APP_TYPE";
        public const string APP_SCOPE = "APP_SCOPE";
        public const string ZONE_ID = "ZONE_ID";

        public static void InitAppEnvironment(IReadOnlyDictionary<string, string?> configuration)
        {
            var appName = configuration["Tny:Net:Name"];
            var serverId = configuration["Tny:Net:ServerId"];
            var appType = configuration["Tny:Net:AppType"];
            var appScope = configuration["Tny:Net:ScopeType"];
            var zoneId = configuration["R2:Common:Zone:MainZone"];
            var appInstance = appName + "-s" + serverId;
            Environment.SetEnvironmentVariable(APP_NAME, appName);
            Environment.SetEnvironmentVariable(SERVER_ID, serverId);
            Environment.SetEnvironmentVariable(APP_INSTANCE, appInstance);
            Environment.SetEnvironmentVariable(APP_TYPE, appType);
            Environment.SetEnvironmentVariable(APP_SCOPE, appScope);
            Environment.SetEnvironmentVariable(ZONE_ID, zoneId);
        }

        public static string GetEnvironment()
        {
            return GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production";
        }

        public static string? GetEnvironmentVariable(string variableName)
        {
            try
            {
                var environmentVariable = Environment.GetEnvironmentVariable(variableName);
                return string.IsNullOrWhiteSpace(environmentVariable) ? default : environmentVariable.Trim();
            } catch (Exception ex)
            {
                var objArray = new object[1] {
                    variableName
                };
                LOGGER.LogError(ex, "Failed to lookup environment variable {0}", objArray);
                return null;
            }
        }
    }

}
