using System;

namespace TnyFramework.Net.Attributes
{

    /// <summary>
    /// 配置生效协议号, 使用在 IAuthenticateValidator 实现类上. 自动关联对应协议
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class AuthProtocolAttribute : Attribute
    {
        public AuthProtocolAttribute(params int[] protocols)
        {
            Protocols = protocols;
            All = false;
        }

        public AuthProtocolAttribute(bool all)
        {
            Protocols = new int[] { };
            All = all;
        }

        /// <summary>
        /// 协议号
        /// </summary>
        public int[] Protocols { get; }

        /// <summary>
        /// 是否是全部
        /// </summary>
        public bool All { get; }
    };

}
