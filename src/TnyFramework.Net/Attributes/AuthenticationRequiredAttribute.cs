// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using TnyFramework.Common.Exceptions;
using TnyFramework.Net.Command.Auth;

namespace TnyFramework.Net.Attributes
{

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthenticationRequiredAttribute : Attribute
    {
        /// <summary>
        /// 必须剩饭认证
        /// </summary>
        /// <param name="contactGroups">限制用户组</param>
        /// <exception cref="CommonException"></exception>
        public AuthenticationRequiredAttribute(params string[] contactGroups)
        {
            Validator = null;
            ContactGroups = contactGroups;
        }

        /// <summary>
        /// 必须剩饭认证
        /// </summary>
        /// <param name="validator">认证器类型, validator必须实现 IAuthenticateValidator 接口 </param>
        /// <param name="contactGroups">限制用户组</param>
        /// <exception cref="CommonException"></exception>
        public AuthenticationRequiredAttribute(Type validator, params string[] contactGroups)
        {
            if (!typeof(IAuthenticationValidator).IsAssignableFrom(validator))
            {
                throw new CommonException($"{validator} 没有继承 {typeof(IAuthenticationValidator)}");
            }
            ContactGroups = contactGroups;
            Validator = validator;
        }

        /// <summary>
        /// 身份验证器类型
        /// </summary>
        public void SetValidator<TValidator>()
            where TValidator : IAuthenticationValidator
        {
            Validator = typeof(TValidator);
        }

        /// <summary>
        /// 身份验证器类型
        /// </summary>
        public Type? Validator { get; private set; }

        /// <summary>
        /// 是否是必须的, 默认为 true
        /// </summary>
        public string[] ContactGroups { get; }

        /// <summary>
        /// 是否生效
        /// </summary>
        public bool Enable { get; } = true;
    }

}
