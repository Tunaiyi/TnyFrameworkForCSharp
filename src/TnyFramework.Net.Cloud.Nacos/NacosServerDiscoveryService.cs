// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nacos.AspNetCore.V2;
using Nacos.V2;
using Nacos.V2.Naming.Core;
using Nacos.V2.Naming.Dtos;
using Newtonsoft.Json;
using TnyFramework.Common.Extensions;
using TnyFramework.Net.Base;

namespace TnyFramework.Net.Cloud.Nacos
{

    public class NacosServerDiscoveryService : INetServerDiscoveryService
    {
        private const string METADATA_NET_VERSION = "DOTNET_VERSION";
        private const string METADATA_HOST_OS = "HOST_OS";

        private const string NET_SERVE_NAME = "net-serveName";
        private const string NET_SERVICE = "net-service";
        private const string NET_SERVER_ID = "net-serverId";
        private const string NET_APP_TYPE = "net-appType";
        private const string NET_SCHEME = "net-scheme";
        private const string NET_SCOPE_TYPE = "net-scopeType";
        private const string NET_METADATA = "net-metadata";

        private readonly ILogger logger;
        private readonly INacosNamingService namingService;
        private readonly NacosAspNetOptions aspNetOptions;

        public NacosServerDiscoveryService(INacosNamingService namingService,
            IOptionsMonitor<NacosAspNetOptions> aspNetOptions,
            ILogger<NacosServerDiscoveryService> logger)
        {
            this.namingService = namingService;
            this.aspNetOptions = aspNetOptions.CurrentValue;
            this.logger = logger;
        }

        public async Task RegisterInstance(INetApplication netApplication, INetServer server)
        {
            Exception? exception = null;
            var instance = CreateInstance(netApplication, server);
            for (var i = 0; i < 3; i++)
            {
                var times = i + 1;
                try
                {
                    logger.LogInformation("register instance to nacos server, 【{Ins}】| times = {Times}", instance, times);
                    await namingService.RegisterInstance(instance.ServiceName, aspNetOptions.GroupName, instance);
                    exception = null;
                    break;
                } catch (Exception ex)
                {
                    logger.LogError(ex, "register instance 【{Ins}】 error | times = {Times}", instance, times);
                    exception = ex;
                }
            }
            if (exception != null)
                throw exception;
        }

        public async Task DeregisterInstance(INetServer server)
        {

            Exception? exception = null;
            var setting = server.Setting;
            for (var i = 0; i < 3; i++)
            {
                var times = i + 1;
                try
                {
                    await namingService
                        .DeregisterInstance(setting.DiscoverService(), aspNetOptions.GroupName, GetIp(setting), GetPort(setting),
                            aspNetOptions.ClusterName)
                        .ConfigureAwait(false);
                    exception = null;
                    break;
                } catch (Exception ex)
                {
                    logger.LogError(ex, "register instance 【{Ins}】 error | times = {Times}", setting.ServiceName(), times);
                    exception = ex;
                }
            }
            if (exception != null)
                throw exception;
        }

        private static string GetIp(IServerSetting setting)
        {
            var host = setting.ServeHost;
            if (host.IsBlank())
            {
                host = setting.Host;
            }
            if (!host.IsBlank() && host != "0.0.0.0" && host != "*")
                return host;
            var name = Dns.GetHostName();
            var ipAddresses = Dns.GetHostAddresses(name);
            foreach (var ipAddress in ipAddresses)
            {
                if (ipAddress.AddressFamily == AddressFamily.InterNetwork)
                {
                    host = ipAddress.ToString();
                }
            }
            return host;
        }

        private static int GetPort(IServerSetting setting)
        {
            var port = setting.ServePort;
            if (port <= 0)
            {
                port = setting.Port;
            }
            return port;
        }

        private Instance CreateInstance(INetApplication netApplication, INetServer server)
        {
            var metadata = new Dictionary<string, string> {
                {PreservedMetadataKeys.REGISTER_SOURCE, "ASPNET_CORE"},
                {METADATA_NET_VERSION, Environment.Version.ToString()},
                {METADATA_HOST_OS, Environment.OSVersion.ToString()},
            };
            foreach (var (key, value) in aspNetOptions.Metadata)
            {
                metadata.TryAdd(key, value);
            }
            var setting = server.Setting;
            var instance = new Instance {
                Ephemeral = true,
                ServiceName = setting.DiscoverService(),
                ClusterName = aspNetOptions.ClusterName,
                Enabled = true,
                Healthy = true,
                Ip = GetIp(setting),
                Port = GetPort(setting),
                Weight = aspNetOptions.Weight,
                Metadata = metadata,
                InstanceId = setting.DiscoverService()
            };
            var appContext = netApplication.AppContext;
            var netMetadata = "{}";
            if (!setting.Metadata.IsEmpty())
            {
                netMetadata = JsonConvert.SerializeObject(setting.Metadata);
            }
            metadata.Add(NET_SERVE_NAME, setting.ServeName);
            metadata.Add(NET_SERVICE, setting.Name);
            metadata.Add(NET_SERVER_ID, appContext.ServerId + "");
            metadata.Add(NET_APP_TYPE, appContext.AppType);
            metadata.Add(NET_SCOPE_TYPE, appContext.ScopeType);
            metadata.Add(NET_SCHEME, setting.Scheme);
            metadata.Add(NET_METADATA, netMetadata);
            return instance;
        }
    }

}
