// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using TnyFramework.Common.Attribute;

namespace TnyFramework.Net.Transport
{

    public interface IConnection : IAddressPeer
    {
        /// <summary>
        /// 是否活跃
        /// </summary>
        /// <returns></returns>
        bool IsActive();

        /// <summary>
        /// 是否关闭会话
        /// </summary>
        /// <returns></returns>
        bool IsClosed();

        /// <summary>
        /// 关闭会话
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
