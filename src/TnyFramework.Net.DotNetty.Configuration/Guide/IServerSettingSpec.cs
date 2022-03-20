namespace TnyFramework.Net.DotNetty.Configuration.Guide
{
    public interface IServerSettingSpec
    {
        ServerSettingSpec ServiceName(string value);

        ServerSettingSpec Host(string value);

        ServerSettingSpec Port(int port);

        ServerSettingSpec Libuv(bool value);
    }
}
