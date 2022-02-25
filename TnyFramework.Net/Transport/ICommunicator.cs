using TnyFramework.Net.Command;
using TnyFramework.Net.Rpc;
namespace TnyFramework.Net.Transport
{
    public interface ICommunicator
    {
        /// <summary>
        /// 获取用户 id
        /// </summary>
        object GetUserId();


        /// <summary>
        /// 用户类型
        /// </summary>
        /// <returns></returns>
        string UserType { get; }


        /// <summary>
        /// 认证
        /// </summary>
        ICertificate GetCertificate();


        /// <summary>
        /// 是否登陆认证
        /// </summary>
        /// <returns></returns>
        bool IsAuthenticated();
    }

    public interface ICommunicator<out TUserId> : ICommunicator
    {
        /// <summary>
        /// 用户 id
        /// </summary>
        TUserId UserId { get; }

        /// <summary>
        /// 认证
        /// </summary>
        ICertificate<TUserId> Certificate { get; }
    }
}
