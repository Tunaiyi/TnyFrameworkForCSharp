using TnyFramework.DI.Units;
using TnyFramework.Net.Endpoint;
namespace TnyFramework.Net.DotNetty.Configuration.Endpoint
{
    public class SessionKeeperSettingSpecs :
        UnitContainerSpec<ISessionKeeperSetting, ISessionKeeperSettingSpec, SessionKeeperSettingSpec, IEndpointUnitContext>
    {
        public SessionKeeperSettingSpecs() : base(SessionKeeperSettingSpec.New)
        {
        }
    }
}
