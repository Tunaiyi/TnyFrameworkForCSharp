// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using NLog;
using NLog.Extensions.Hosting;
using NLog.Extensions.Logging;
using TnyFramework.Extensions.Configuration;
using LogFactory = TnyFramework.Common.Logger.LogFactory;

namespace TnyFramework.NLog.Hosting.Extensions;

public static class NLogExtensions
{
    public static IHostBuilder UseNLogForApplication(this IHostBuilder builder, Action<IConfigurationBuilder>? loadFile = null)
    {
        return builder
            .ConfigureHostConfiguration(configureBuilder => {
                loadFile?.Invoke(configureBuilder);
                UseNLogForApplication(configureBuilder);
                LogFactory.DefaultFactory = new NLogLoggerFactory();
            })
            .UseNLog();
    }

    private static void UseNLogForApplication(IConfigurationBuilder configureBuilder)
    {
        var root = configureBuilder.Build();
        SetupNLog(root);
        ConfigSettingLayoutRenderer.DefaultConfiguration = root;
    }

    private static void SetupNLog(IConfiguration configuration, string nlogConfigSection = "NLog", string? basePath = null)
    {
        var setupBuilder = LogManager.Setup();
        if (!string.IsNullOrEmpty(nlogConfigSection))
        {
            var section = configuration.GetSection(nlogConfigSection);
            if ((section.GetChildren().Any() ? 1 : 0) != 0)
            {
                setupBuilder
                    .LoadConfigurationFromSection(configuration, nlogConfigSection)
                    .GetCurrentClassLogger();
                return;
            }

        }
        setupBuilder.SetupExtensions(e => e.RegisterConfigSettings(configuration));
        var environment = TnyEnvironments.GetEnvironment();
        if (!string.IsNullOrEmpty(basePath))
        {
            if (!string.IsNullOrEmpty(environment))
                setupBuilder.LoadConfigurationFromFile(Path.Combine(basePath, "nlog." + environment + ".config"));
            setupBuilder.LoadConfigurationFromFile(Path.Combine(basePath, "nlog.config"));
        } else if (!string.IsNullOrEmpty(environment))
        {
            setupBuilder.LoadConfigurationFromFile("nlog." + environment + ".config");
        }
        setupBuilder.LoadConfigurationFromFile().GetCurrentClassLogger();
    }
}
