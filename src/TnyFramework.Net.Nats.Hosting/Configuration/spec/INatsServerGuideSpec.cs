using System;
using NATS.Client.Core;
using TnyFramework.Net.Nats.Options;
using TnyFramework.Net.Nats.Transports;

namespace TnyFramework.Net.Nats.Hosting.Configuration.spec;

public interface
    INatsServerGuideSpec : INatsGuideSpec<INatsServerGuide, INatsServerGuideUnitContext, INatsServerGuideSpec>
{
    /// <summary>
    /// 设置服务配置
    /// </summary>
    /// <param name="setting">配置</param>
    INatsServerGuideSpec Server(INatsServerSetting setting);

    INatsServerGuideSpec Host(string value);

    INatsServerGuideSpec Port(int port);

    INatsServerGuideSpec ConfigureServer(Action<INatsServerSettingSpec> action);

    INatsServerGuideSpec ConfigureNats(Action<NatsOpts> action);
}