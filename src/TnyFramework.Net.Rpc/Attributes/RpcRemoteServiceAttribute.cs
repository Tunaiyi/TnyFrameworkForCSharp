using System;

namespace TnyFramework.Net.Rpc.Attributes
{

    /// <summary>
    /// 客户端远程服务
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Interface | AttributeTargets.Class)]
    public class RpcRemoteServiceAttribute : Attribute
    {
        /// <summary>
        /// 调用服务名
        /// </summary>
        public string Service { get; }

        /// <summary>
        /// 代理服务
        /// </summary>
        public string ForwardService { get; }

        public RpcRemoteServiceAttribute(string service, string forwardService = "")
        {
            Service = service;
            ForwardService = forwardService;
        }
    }

}
