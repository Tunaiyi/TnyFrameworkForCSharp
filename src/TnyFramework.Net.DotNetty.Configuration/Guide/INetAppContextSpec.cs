namespace TnyFramework.Net.DotNetty.Configuration.Guide
{
    public interface INetAppContextSpec
    {
        NetAppContextSpec ServerId(int value);

        NetAppContextSpec AppName(string value);

        NetAppContextSpec AppType(string value);

        NetAppContextSpec ScopeType(string value);

        NetAppContextSpec Locale(string value);
    }
}
