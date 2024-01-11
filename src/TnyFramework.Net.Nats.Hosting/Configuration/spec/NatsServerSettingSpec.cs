using System;
using TnyFramework.DI.Units;
using TnyFramework.Net.Nats.Options;

namespace TnyFramework.Net.Nats.Hosting.Configuration.spec;

public class NatsServerSettingSpec : UnitSpec<INatsServerSetting, object>, INatsServerSettingSpec
{
    private NatsServerSetting Setting { get; } = new();

    public NatsServerSettingSpec(string unitName = "") : base(unitName)
    {
        Default(_ => Setting);
    }

    /// <summary>
    /// 设置
    /// </summary>
    /// <param name="setting"></param>
    /// <returns></returns>
    public INatsServerSettingSpec ConfigureServer(INatsServerSetting setting)
    {
        Creator(_ => setting);
        return this;
    }

    /// <summary>
    /// 设置
    /// </summary>
    /// <param name="factory"></param>
    /// <returns></returns>
    public INatsServerSettingSpec ConfigureServer(Func<INatsServerSetting> factory)
    {
        Creator(_ => factory());
        return this;
    }

    public INatsServerSettingSpec ConfigureServer(Action<NatsServerSetting> action)
    {
        action(Setting);
        return this;
    }

    public INatsServerSettingSpec ConfigureRpcOptions(Action<PubSubRpcOptions> action)
    {
        action(Setting.RpcOptions);
        return this;
    }

    /// <summary>
    /// 设置主机名(域名/IP)
    /// </summary>
    /// <param name="value">域名</param>
    /// <returns></returns>
    public INatsServerSettingSpec Host(string value)
    {
        Setting.Host = value;
        return this;
    }

    /// <summary>
    /// 设置端口
    /// </summary>
    /// <param name="port"></param>
    /// <returns></returns>
    public INatsServerSettingSpec Port(int port)
    {
        Setting.Port = port;
        return this;
    }

    /// <summary>
    /// 设置服务发现名
    /// </summary>
    /// <param name="value">服务发现名</param>
    /// <returns></returns>
    public INatsServerSettingSpec ServeName(string value)
    {
        Setting.ServeName = value;
        return this;
    }

    /// <summary>
    /// 设置服务名
    /// </summary>
    /// <param name="value">服务名</param>
    /// <returns></returns>
    public INatsServerSettingSpec ServiceName(string value)
    {
        Setting.Name = value;
        return this;
    }
}