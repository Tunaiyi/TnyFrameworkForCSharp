using TnyFramework.Common.Extension;
namespace TnyFramework.Net.Base
{
    public static class ServerSettingExtensions
    {
        public static string ServiceName(this IServerSetting setting)
        {
            return setting.Name.IsBlank() ? setting.ServeName : setting.Name;
        }


        public static string DiscoverService(this IServerSetting setting)
        {
            return setting.ServeName.IsBlank() ? setting.Name : setting.ServeName;
        }
    }
}
