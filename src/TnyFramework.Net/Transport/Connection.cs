using System.Net;
using TnyFramework.Common.Attribute;
namespace TnyFramework.Net.Transport
{
    public interface IConnection
    {
        /// <summary>
        /// 远程地址
        /// </summary>
        EndPoint RemoteAddress { get; }

        /// <summary>
        /// 本地地址
        /// </summary>
        EndPoint LocalAddress { get; }


        /// <summary>
        /// 是否活跃
        /// </summary>
        /// <returns></returns>
        bool IsActive();


        /// <summary>
        /// 是否关闭终端
        /// </summary>
        /// <returns></returns>
        bool IsClosed();


        /// <summary>
        /// 关闭终端
        /// </summary>
        /// <returns></returns>
        bool Close();


        /// <summary>
        /// 获取属性
        /// </summary>
        /// <returns></returns>
        IAttributes Attributes { get; }
    }
}
