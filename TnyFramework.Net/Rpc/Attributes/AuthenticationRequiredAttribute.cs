using System;
using TnyFramework.Common.Exception;
namespace TnyFramework.Net.Rpc.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthenticationRequiredAttribute : Attribute
    {
        /// <summary>
        /// 必须剩饭认证
        /// </summary>
        /// <param name="validator">认证器类型, validator必须实现 IAuthenticateValidator 接口 </param>
        /// <param name="userGroups">限制用户组</param>
        /// <exception cref="CommonException"></exception>
        public AuthenticationRequiredAttribute(params string[] userGroups)
        {
            UserGroups = userGroups;
        }


        /// <summary>
        /// 必须剩饭认证
        /// </summary>
        /// <param name="validator">认证器类型, validator必须实现 IAuthenticateValidator 接口 </param>
        /// <param name="userGroups">限制用户组</param>
        /// <exception cref="CommonException"></exception>
        public AuthenticationRequiredAttribute(Type validator, params string[] userGroups)
        {
            if (!typeof(IAuthenticateValidator).IsAssignableFrom(validator))
            {
                throw new CommonException($"{validator} 没有继承 {typeof(IAuthenticateValidator)}");
            }
            UserGroups = userGroups;
            Validator = validator;
        }


        /// <summary>
        /// 身份验证器类型
        /// </summary>
        public void SetValidator<TValidator>()
            where TValidator : IAuthenticateValidator
        {
            Validator = typeof(TValidator);
        }


        /// <summary>
        /// 身份验证器类型
        /// </summary>
        public Type Validator { get; private set; }

        /// <summary>
        /// 是否是必须的, 默认为 true
        /// </summary>
        public string[] UserGroups { get; }

        /// <summary>
        /// 是否生效
        /// </summary>
        public bool Enable { get; } = true;
    }
}
