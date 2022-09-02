using TnyFramework.Net.Endpoint;
using TnyFramework.Net.Transport;

namespace TnyFramework.Net.Rpc
{

    public interface IRpcRemoteAccess : ISender
    {
        /// <summary>
        /// 访问点 id
        /// </summary>
        long AccessId { get; }

        /// <summary>
        /// 是否已上线
        /// </summary>
        /// <returns>连接返回true 否则返回false</returns>
        bool IsActive();

        /// <summary>
        /// endpoint
        /// </summary>
        IEndpoint Endpoint { get; }
    }

}
