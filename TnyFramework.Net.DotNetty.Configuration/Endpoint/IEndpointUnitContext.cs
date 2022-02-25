using System.Collections.Generic;
using TnyFramework.Net.Endpoint;
namespace TnyFramework.Net.DotNetty.Configuration.Endpoint
{
    public interface IEndpointUnitContext
    {
        IEndpointKeeperManager LoadEndpointKeeperManager();

        ISessionKeeperSetting LoadDefaultSessionKeeperSetting();

        IList<ISessionKeeperSetting> LoadCustomSessionKeeperSettings();

        IDictionary<string, ISessionFactory> LoadSessionFactories();

        IDictionary<string, ISessionKeeperFactory> LoadSessionKeeperFactories();
    }
}
