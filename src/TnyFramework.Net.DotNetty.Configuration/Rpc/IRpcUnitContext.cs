using TnyFramework.Net.DotNetty.Configuration.Guide;
using TnyFramework.Net.Rpc.Auth;

namespace TnyFramework.Net.DotNetty.Configuration.Rpc
{

    public interface IRpcUnitContext
    {
        IRpcAuthService LoadRpcAuthService();

        IRpcUserPasswordManager LoadRpcUserPasswordManager();

        NetUnitContext NetUnitContext { get; }
    }

}
