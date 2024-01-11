using System;
using TnyFramework.Net.Nats.Options;

namespace TnyFramework.Net.Nats.Hosting.Configuration.spec;

public interface INatsServerSettingSpec
{
    INatsServerSettingSpec Host(string value);

    INatsServerSettingSpec Port(int port);

    /// <summary>
    /// 设置名
    /// </summary>
    /// <param name="value">服务名</param>
    /// <returns></returns>
    INatsServerSettingSpec ServiceName(string value);

    /// <summary>
    /// 设置服务发现名
    /// </summary>
    /// <param name="value">服务发现名</param>
    /// <returns></returns>
    INatsServerSettingSpec ServeName(string value);

    /// <summary>
    /// 设置
    /// </summary>
    /// <param name="setting"></param>
    /// <returns></returns>
    INatsServerSettingSpec ConfigureServer(INatsServerSetting setting);

    /// <summary>
    /// 设置
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    INatsServerSettingSpec ConfigureServer(Action<NatsServerSetting> action);

    /// <summary>
    /// 设置
    /// </summary>
    /// <param name="factory"></param>
    /// <returns></returns>
    INatsServerSettingSpec ConfigureServer(Func<INatsServerSetting> factory);

    /// <summary>
    /// 设置
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    INatsServerSettingSpec ConfigureRpcOptions(Action<PubSubRpcOptions> action);
}