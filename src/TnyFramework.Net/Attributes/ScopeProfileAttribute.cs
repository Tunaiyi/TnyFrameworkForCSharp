using System;

namespace TnyFramework.Net.Attributes
{

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class ScopeProfileAttribute : Attribute
    {
        public ScopeProfileAttribute(params string[] scopes)
        {
            Scopes = scopes;
        }

        /// <summary>
        /// 限制的 作用域范围
        /// </summary>
        public string[] Scopes { get; }
    }

}
